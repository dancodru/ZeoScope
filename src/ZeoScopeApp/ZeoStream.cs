//Copyright 2011 dancodru

//Licensed under the Apache License, Version 2.0 (the "License");
//You may not use this file except in compliance with the License.
//You may obtain a copy of the License at
 
//   http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.IO.Ports;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    #region Enums and Helper Classes
    internal enum ZeoDataType
    {
        Event = 0x00,
        SliceEnd = 0x02,
        Version = 0x03,
        Waveform = 0x80,
        FrequencyBins = 0x83,
        SQI = 0x84,
        ZeoTimestamp = 0x8A,
        Impedance = 0x97,
        BadSignal = 0x9C,
        SleepStage = 0x9D,
        Error = 0xFF,
    }

    /// <summary>
    /// Additional DataTypes used by ZeoScope
    /// </summary>
    internal enum Z9DataType
    {
        SoundAlarmVolume = 0x00,
        SoundAlarmEnabled = 0x01,
    }

    internal enum ZeoSleepStage
    {
        Undefined0 = 0,
        Awake = 1,
        REM = 2,
        Light = 3,
        Deep = 4,
        Undefined = 5,
        Sleep = 6,  // REM || Light || Deep
    }

    internal enum ZeoEvent
    {
        NoEvent = 0x00,
        NightStart = 0x05,
        SleepOnset = 0x07,
        HeadbandDocked = 0x0E,
        HeadbandUnDocked = 0x0F,
        AlarmOff = 0x10,
        AlarmSnooze = 0x11,
        AlarmPlay = 0x13,
        NightEnd = 0x15,
        NewHeadband = 0x24,
    }

    internal class ChannelData
    {
        public ChannelData(int n)
        {
            this.Values = new float[n];
        }

        public float[] Values { get; set; }
    }

    internal class ZeoMessage
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public static readonly int SamplesPerMessage = 128;

        public static readonly int FrequencyBinsLength = 7;

        public static readonly int MinSoundVolume = -10000;

        public ZeoMessage()
        {
            this.SoundAlarmVolume = ZeoMessage.MinSoundVolume;
        }

        public float[] Waveform { get; set; }

        public float? Impedance { get; set; }

        public ZeoEvent? Event { get; set; }

        public int ZeoTimestamp { get; set; }

        public bool? BadSignal { get; set; }

        public int? SQI { get; set; }

        public float[] FrequencyBins { get; set; }

        public ZeoSleepStage? SleepStage { get; set; }

        public float Second { get; set; }

        public int SoundAlarmVolume { get; set; }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.Waveform != null)
            {
                stringBuilder.AppendFormat("Waveform; Second: {0:.00000}\r\n", this.Second);
            }

            if (this.Impedance != null)
            {
                stringBuilder.AppendFormat("Impedance: {0:0.000}\r\n", this.Impedance);
            }

            if (this.Event != null)
            {
                stringBuilder.AppendFormat("Event: {0}\r\n", Enum.GetName(typeof(ZeoEvent), this.Event));
            }

            stringBuilder.AppendFormat("ZeoTimestamp: {0}\r\n", UnixEpoch.AddSeconds(this.ZeoTimestamp));

            if (this.BadSignal != null)
            {
                stringBuilder.AppendFormat("BadSignal: {0}\r\n", this.BadSignal);
            }

            if (this.SQI != null)
            {
                stringBuilder.AppendFormat("SQI: {0}\r\n", this.SQI);
            }

            if (this.FrequencyBins != null)
            {
                stringBuilder.Append("FrequencyBins: ");
                for (int i = 0; i < this.FrequencyBins.Length; i++)
                {
                    stringBuilder.AppendFormat("{0:0.00} ", this.FrequencyBins[i]);
                }

                stringBuilder.AppendLine();
            }

            if (this.SleepStage != null)
            {
                stringBuilder.AppendFormat("SleepStage: {0}", Enum.GetName(typeof(ZeoSleepStage), this.SleepStage));
            }

            return stringBuilder.ToString().Trim();
        }
    }
    #endregion

    internal class ZeoStream : IDisposable
    {
        #region Fields
        public static readonly double SamplesPerSec = 128.0;
        public static readonly int SleepStageSeconds = 30;

        private static string versionHeader = "ZeoVersion";
        private static string versionFormat = versionHeader + ": {0:00}.{1:00}.{2:0000}.{3:0000}";

        private static string[] supportedVersions = new string[] {
            string.Empty, // old version (no version string)
            "ZeoVersion: 00.09.0000.0000"
        };

        private string fileName;

        private byte[] buffer = new byte[512];
        private List<ZeoMessage> zeoMessages = new List<ZeoMessage>(100);
        private bool writeEnabled = false;

        private SerialPort serialPort;
        private FileStream binFile;

        private Thread readThread;
        private ManualResetEvent exitWorkerEvent;
        private ReaderWriterLock rwLock = new ReaderWriterLock();

        private Stopwatch stopWatch = new Stopwatch();

        private SoundAlarm soundAlarm;
        #endregion

        #region Constructor and Properties
        public ZeoStream(ManualResetEvent exitWorkerEvent)
        {
            this.exitWorkerEvent = exitWorkerEvent;
        }

        public event EventHandler HeadbandDocked;

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        public int Length
        {
            get
            {
                this.rwLock.AcquireReaderLock(Timeout.Infinite);
                int count = this.zeoMessages.Count;
                this.rwLock.ReleaseLock();

                return count;
            }
        }

        public bool LiveStream { get; private set; }

        public bool SoundAlarmEnabled { get; private set; }
        #endregion

        #region Public Methods
        public void OpenLiveStream(string comPortName, string fileName, SoundAlarm soundAlarm)
        {
            Directory.CreateDirectory("ZeoData");

            this.soundAlarm = soundAlarm;

            this.LiveStream = true;
            this.fileName = string.Format(@"ZeoData\{0}_{1}.zeo", fileName, DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            this.binFile = new FileStream(this.fileName, FileMode.CreateNew);
            this.AddVersionString();

            if (this.soundAlarm != null)
            {
                this.WriteZ9Message(new ZeoMessage(), Z9DataType.SoundAlarmEnabled);
            }

            this.serialPort = new SerialPort(comPortName, 38400, Parity.None, 8, StopBits.One);
            this.serialPort.Open();
            this.serialPort.DiscardInBuffer();

            this.readThread = new Thread(new ThreadStart(this.ReadSerialStream));
            this.readThread.Start();
        }

        public void OpenFileStream(string fileName)
        {
            this.fileName = fileName;

            this.binFile = new FileStream(this.fileName, FileMode.Open);
            this.VerifyVersionString();

            try
            {
                while (true)
                {
                    ZeoMessage zeoMessage = this.ReadMessage();
                    if (zeoMessage != null && zeoMessage.Waveform != null)
                    {
                        break;
                    }
                }

                while (true)
                {
                    ZeoMessage zeoMessage = this.ReadMessage();
                    if (zeoMessage != null)
                    {
                        this.zeoMessages.Add(zeoMessage);
                        if (zeoMessage.Event == ZeoEvent.HeadbandDocked)
                        {
                            break;
                        }
                    }
                }
            }
            catch (IOException)
            {
            }
            finally
            {
                if (this.binFile != null)
                {
                    this.binFile.Close();
                    this.binFile = null;
                }
            }
        }

        public ChannelData[] ReadFrequencyDataFromLastPosition(ref int lastPosition, int len)
        {
            this.rwLock.AcquireReaderLock(Timeout.Infinite);

            ChannelData[] freqData = new ChannelData[len];

            for (int i = lastPosition, j = 0; i < (lastPosition + len) && i < this.zeoMessages.Count; i++, j++)
            {
                ZeoMessage zeoMessage = this.zeoMessages[i];
                freqData[j] = new ChannelData(ZeoMessage.FrequencyBinsLength + 2);
                if (zeoMessage.FrequencyBins != null)
                {
                    int k = 0;
                    for (k = 0; k < zeoMessage.FrequencyBins.Length; k++)
                    {
                        freqData[j].Values[k] = zeoMessage.FrequencyBins[k];
                    }

                    freqData[j].Values[k] = zeoMessage.Impedance ?? 0;
                }
                else
                {
                    freqData[j].Values[freqData[j].Values.Length - 2] = zeoMessage.Impedance ?? 0;
                }

                freqData[j].Values[freqData[j].Values.Length - 1] = zeoMessage.SoundAlarmVolume;
            }

            if (this.LiveStream)
            {
                lastPosition = this.Length - len;
                if (lastPosition < 0)
                {
                    lastPosition = 0;
                }
            }

            this.rwLock.ReleaseLock();

            return freqData;
        }

        public ChannelData[] ReadStageDataFromLastPosition(ref int lastPosition, int len)
        {
            this.rwLock.AcquireReaderLock(Timeout.Infinite);

            ChannelData[] stageData = new ChannelData[len];

            for (int i = lastPosition, j = 0; j < len && i < this.zeoMessages.Count; i++)
            {
                ZeoMessage zeoMessage = this.zeoMessages[i];
                stageData[j] = new ChannelData(2);
                if (zeoMessage.SleepStage != null)
                {
                    stageData[j].Values[0] = -(int)zeoMessage.SleepStage;
                    stageData[j].Values[1] = zeoMessage.SoundAlarmVolume;
                    j++;
                }
            }

            for (int i = 0; i < len; i++)
            {
                if (stageData[i] == null)
                {
                    stageData[i] = new ChannelData(2);
                    stageData[i].Values[0] = -5;
                    stageData[i].Values[1] = ZeoMessage.MinSoundVolume;
                }
                else if (stageData[i].Values[0] == 0)
                {
                    stageData[i].Values[0] = -5;
                    stageData[i].Values[1] = ZeoMessage.MinSoundVolume;
                }
            }

            this.rwLock.ReleaseLock();

            return stageData;
        }

        public ChannelData[] ReadEegFromLastPosition(ref int lastPosition, int len)
        {
            this.rwLock.AcquireReaderLock(Timeout.Infinite);

            ChannelData[] eegValues = new ChannelData[len];
            for (int i = 0; i < len; i++)
            {
                eegValues[i] = new ChannelData(1);
            }

            int count = (len / ZeoMessage.SamplesPerMessage) + 1;
            for (int i = lastPosition, j = 0; i < (lastPosition + count) && i < this.zeoMessages.Count; i++)
            {
                ZeoMessage zeoMessage = this.zeoMessages[i];
                for (int k = 0; j < len && k < ZeoMessage.SamplesPerMessage; j++, k++)
                {
                    if (zeoMessage.Waveform != null)
                    {
                        eegValues[j].Values[0] = zeoMessage.Waveform[k];
                    }
                    else
                    {
                        eegValues[j].Values[0] = 0;
                    }
                }
            }

            if (this.LiveStream)
            {
                lastPosition = ((this.Length * ZeoMessage.SamplesPerMessage) - len) / ZeoMessage.SamplesPerMessage;
                if (lastPosition < 0)
                {
                    lastPosition = 0;
                }
            }

            ChannelData[] filteredValues = this.Filter50Hz(eegValues);
            for (int i = 0; i < len; i++)
            {
                eegValues[i] = filteredValues[i];
            }

            this.rwLock.ReleaseLock();

            return eegValues;
        }

        public DateTime? GetTimeFromIndex(int index)
        {
            if (index >= 0 && this.zeoMessages.Count > index)
            {
                // DirectX switches the FPU to single precision instead of double
                // Hence this work around to have 1 second precision
                // Web search: "DirectX single precision"
                int seconds = this.zeoMessages[index].ZeoTimestamp;
                uint step = 10000;
                DateTime d = ZeoMessage.UnixEpoch;

                for (uint i = 0; i < seconds / step; i++)
                {
                    d = d.AddSeconds(step);
                }

                d = d.AddSeconds(seconds % step);

                return d;
            }
            else
            {
                if (this.LiveStream == true)
                {
                    return DateTime.Now;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Dispose()
        {
            try
            {
                if (this.binFile != null)
                {
                    this.binFile.Close();
                    this.binFile = null;
                }

                if (this.serialPort != null)
                {
                    this.serialPort.Close();
                    this.serialPort = null;
                }
            }
            catch (IOException)
            {
            }
        }
        #endregion

        #region Private Methods
        private void ReadSerialStream()
        {
            Thread.CurrentThread.Name = "ReadSerialStream";

            this.writeEnabled = false;

            while (this.exitWorkerEvent.WaitOne(0, true) == false)
            {
                try
                {
                    if (this.writeEnabled == false)
                    {
                        ZeoMessage zeoMessage = this.ReadMessage();

                        if (zeoMessage.Waveform != null)
                        {
                            this.writeEnabled = true;
                        }
                    }

                    if (this.writeEnabled == true)
                    {
                        ZeoMessage zeoMessage = this.ReadMessage();

                        if (zeoMessage != null)
                        {
                            this.rwLock.AcquireWriterLock(Timeout.Infinite);
                            this.zeoMessages.Add(zeoMessage);
                            this.rwLock.ReleaseLock();

                            if (this.soundAlarm != null)
                            {
                                ZeoMessage zm = this.soundAlarm.ProcessZeoMessage(zeoMessage);
                                if (zm != null)
                                {
                                    this.WriteZ9Message(zm, Z9DataType.SoundAlarmVolume);
                                }
                            }

                            if (zeoMessage.Event == ZeoEvent.HeadbandDocked)
                            {
                                if (this.HeadbandDocked != null)
                                {
                                    this.HeadbandDocked(this, null);
                                }

                                this.binFile.Flush();
                                break;
                            }
                        }
                    }
                }
                catch (TimeoutException)
                {
                    // Port Timeout
                }
                catch (InvalidOperationException)
                {
                    // Port closed
                }
                catch (NullReferenceException)
                {
                    // Port disposed
                }
                catch (IOException)
                {
                }
            }

            this.LiveStream = false;
        }

        private void AddVersionString()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionString = string.Format(ZeoStream.versionFormat, version.Major, version.Minor, version.Build, version.Revision);
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(versionString);
            this.binFile.Write(bytes, 0, bytes.Length);
        }

        private void VerifyVersionString()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string currentVersionString = string.Format(ZeoStream.versionFormat, version.Major, version.Minor, version.Build, version.Revision);
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(currentVersionString);

            this.binFile.Read(bytes, 0, bytes.Length);

            string fileVersion = ASCIIEncoding.ASCII.GetString(bytes);
            if (fileVersion.StartsWith(ZeoStream.versionHeader) == false)
            {
                // Old file types
                fileVersion = string.Empty;
            }

            if (currentVersionString != fileVersion)
            {
                bool match = false;
                foreach (string ver in ZeoStream.supportedVersions)
                {
                    if (ver == fileVersion)
                    {
                        match = true;
                        break;
                    }
                }

                if (match == false)
                {
                    this.Dispose();
                    throw new ZeoException("Unsupported File Version: {0}\r\nCurrent Version: {1:00}.{2:00}.{3:0000}.{4:0000}",
                        fileVersion,
                        version.Major, version.Minor, version.Build, version.Revision);
                }
            }
        }

        private ZeoMessage ReadMessage()
        {
            ZeoMessage zeoMessage = new ZeoMessage();

            // A4 are standard Zeo messages
            bool isA4 = false;
            
            // Z9 are additional messages from ZeoScope
            bool isZ9 = false; 

            byte header = 0;
            for (int j = 0; j < 20; j++)
            {
                int i;
                for (i = 0; i < 500; i++)
                {
                    if (header == 'A')
                    {
                        header = this.ReadByte();

                        if (header == '4')
                        {
                            isA4 = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (header == 'Z')
                    {
                        header = this.ReadByte();

                        if (header == '9')
                        {
                            isZ9 = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }


                    header = this.ReadByte();
                }

                if (i != 1)
                {
                    // TODO: log error
                }

                if (isA4 == true)
                {
                    ZeoDataType dataType = this.ReadA4Message(zeoMessage);
                    if (dataType == ZeoDataType.SliceEnd)
                    {
                        return zeoMessage;
                    }
                    else if (dataType == ZeoDataType.Error)
                    {
                        return null;
                    }
                }
                else if (isZ9 == true)
                {
                    this.ReadZ9Message();
                    return null;
                }
            }

            return null;
        }

        private ZeoDataType ReadA4Message(ZeoMessage zeoMessage)
        {
            byte checkSum = this.ReadByte();

            this.ReadNBytes(2);
            int length = this.buffer[0] | this.buffer[1] << 8;

            this.ReadNBytes(2);
            int lengthN = this.buffer[0] | this.buffer[1] << 8;

            if ((short)length != (short)(~lengthN))
            {
                // TODO: log error
                return ZeoDataType.Error;
            }

            byte unixT = this.ReadByte();

            this.ReadNBytes(2);
            float subsecondT = (this.buffer[0] | this.buffer[1] << 8) / 65535.0f;

            byte sequence = this.ReadByte();

            ZeoDataType dataType = (ZeoDataType)this.ReadByte();

            this.ReadNBytes(length - 1);

            switch (dataType)
            {
                case ZeoDataType.FrequencyBins:
                    short[] shorts = new short[7];
                    Buffer.BlockCopy(this.buffer, 0, shorts, 0, 14);
                    float[] floats = new float[7];
                    for (int k = 0; k < 7; k++)
                    {
                        floats[k] = (shorts[k] / 32767.0f) * 100.0f;
                    }

                    zeoMessage.FrequencyBins = floats;
                    break;

                case ZeoDataType.Waveform:
                    shorts = new short[128];
                    Buffer.BlockCopy(this.buffer, 0, shorts, 0, 256);

                    floats = new float[128];
                    for (int k = 0; k < 128; k++)
                    {
                        floats[k] = (shorts[k] / 32767.0f) * 315.0f;
                    }

                    zeoMessage.Waveform = floats;
                    zeoMessage.Second = unixT + subsecondT;

                    // 128 16-bit samples per ~1.005 sec
                    break;

                case ZeoDataType.ZeoTimestamp:
                    zeoMessage.ZeoTimestamp = this.GetInt32();
                    break;

                case ZeoDataType.Event:
                    zeoMessage.Event = (ZeoEvent)this.GetInt32();
                    break;

                case ZeoDataType.Version:
                    break;

                case ZeoDataType.SQI:
                    zeoMessage.SQI = this.GetInt32();
                    break;

                case ZeoDataType.BadSignal:
                    zeoMessage.BadSignal = this.GetInt32() == 0 ? false : true;
                    break;

                case ZeoDataType.Impedance:
                    zeoMessage.Impedance = this.GetImpedance();
                    break;

                case ZeoDataType.SleepStage:
                    zeoMessage.SleepStage = (ZeoSleepStage)this.GetInt32();
                    break;

                case ZeoDataType.SliceEnd:
                    break;

                default:
                    return ZeoDataType.Error;
            }

            return dataType;
        }

        private void ReadZ9Message()
        {
            this.ReadNBytes(2);
            int length = this.buffer[0] | this.buffer[1] << 8;

            // ZeoTimestamp is used as an index to update ZeoMessage in the list
            this.ReadNBytes(4);
            uint zeoTimestamp = (uint)this.GetInt32();
            ZeoMessage zeoMessage = this.GetZeoMessage(zeoTimestamp);

            Z9DataType dataType = (Z9DataType)this.ReadByte();

            this.ReadNBytes(length);

            switch (dataType)
            {
                case Z9DataType.SoundAlarmVolume:
                    zeoMessage.SoundAlarmVolume = this.GetInt32();
                    break;
                case Z9DataType.SoundAlarmEnabled:
                    this.SoundAlarmEnabled = this.buffer[0] != 0;
                    break;
            }
        }

        private void WriteZ9Message(ZeoMessage zeoMessage, Z9DataType dataType)
        {
            short[] lengths = new short[] { 4, 1 };

            if (this.binFile != null && this.binFile.CanWrite == true)
            {
                byte[] bytes = new byte[] { (byte)'Z', (byte)'9' };
                this.binFile.Write(bytes, 0, bytes.Length);

                this.WriteInt16(lengths[(int)dataType]);
                this.WriteInt32(zeoMessage.ZeoTimestamp);

                this.binFile.WriteByte((byte)dataType);

                switch (dataType)
                {
                    case Z9DataType.SoundAlarmVolume:
                        {
                            this.WriteInt32(zeoMessage.SoundAlarmVolume);
                            break;
                        }
                    case Z9DataType.SoundAlarmEnabled:
                        {
                            this.binFile.WriteByte(1);
                            break;
                        }
                }
            }
        }

        private void WriteInt32(int n)
        {
            int[] ints = new int[] { n };
            byte[] bytes = new byte[4];
            Buffer.BlockCopy(ints, 0, bytes, 0, 4);
            this.binFile.Write(bytes, 0, bytes.Length);
        }

        private void WriteInt16(short s)
        {
            short[] shorts = new short[] { s };
            byte[] bytes = new byte[2];
            Buffer.BlockCopy(shorts, 0, bytes, 0, 2);
            this.binFile.Write(bytes, 0, bytes.Length);
        }

        private ZeoMessage GetZeoMessage(uint zeoTimestamp)
        {
            this.rwLock.AcquireReaderLock(Timeout.Infinite);

            // start from the end
            for (int i = this.zeoMessages.Count - 1; i >= 0; i--)
            {
                if (this.zeoMessages[i].ZeoTimestamp == zeoTimestamp)
                {
                    return this.zeoMessages[i];
                }
            }

            this.rwLock.ReleaseLock();

            return new ZeoMessage();
        }

        private int GetInt32()
        {
            int[] ints = new int[1];
            Buffer.BlockCopy(this.buffer, 0, ints, 0, 4);
            return ints[0];
        }

        private float GetImpedance()
        {
            ushort[] shorts = new ushort[2];
            Buffer.BlockCopy(this.buffer, 0, shorts, 0, 4);
            shorts[0] -= 0x8000;
            shorts[1] -= 0x8000;
            if (shorts[0] != 0x7fff)
            {
                float imp = (float)Math.Sqrt((shorts[0] * shorts[0]) + (shorts[1] * shorts[1]));
                return float.IsNaN(imp) == false ? imp : 0;
            }
            else
            {
                return 0;
            }
        }

        private void ReadNBytes(int count)
        {
            if (this.LiveStream == true)
            {
                int n = 0;
                do
                {
                    n += this.serialPort.Read(this.buffer, n, count - n);
                }
                while (n < count);

                if (this.writeEnabled == true)
                {
                    this.binFile.Write(this.buffer, 0, count);
                }
            }
            else
            {
                this.binFile.Read(this.buffer, 0, count);
            }
        }

        private byte ReadByte()
        {
            if (this.LiveStream == true)
            {
                byte b = (byte)this.serialPort.ReadByte();

                if (this.writeEnabled == true)
                {
                    this.binFile.WriteByte(b);
                }

                return b;
            }
            else
            {
                int b = this.binFile.ReadByte();
                if (b == -1)
                {
                    throw new IOException("End of File");
                }

                return (byte)b;
            }
        }

        private ChannelData[] Filter50Hz(ChannelData[] signal)
        {
            // 50Hz Filter based on http://blog.myzeo.com/zeo-raw-data-library-free-your-mind/
            // Convolution math based on http://www.phys.uu.nl/~haque/computing/WPark_recipes_in_python.html
            float[] filter = { 
                0.0032f, 0.0063f, -0.0088f, -0.0000f, 0.0100f, -0.0082f, -0.0047f, 0.0132f, -0.0054f, -0.0108f, 0.0151f, 0.0000f, 
                -0.0177f, 0.0147f, 0.0087f, -0.0248f, 0.0105f, 0.0215f, -0.0315f, -0.0000f, 0.0411f, -0.0369f, -0.0241f, 0.0790f, 
                -0.0404f, -0.1123f, 0.2939f, 0.6250f, 0.2939f, -0.1123f, -0.0404f, 0.0790f, -0.0241f, -0.0369f, 0.0411f, -0.0000f, 
                -0.0315f, 0.0215f, 0.0105f, -0.0248f, 0.0087f, 0.0147f, -0.0177f, 0.0000f, 0.0151f, -0.0108f, -0.0054f, 0.0132f, 
                -0.0047f, -0.0082f, 0.0100f                             
                             };

            int filterLen = filter.Length;
            int len = signal.Length;
            int n = len + filterLen - 1;

            ChannelData[] filteredSignal = new ChannelData[n];
            for (int i = 0; i < n; i++)
            {
                float t = 0;
                int lower = Math.Max(0, i - (filterLen - 1));
                int upper = Math.Min(len - 1, i);

                for (int j = lower; j <= upper; j++)
                {
                    t = t + (signal[j].Values[0] * filter[i - j]);
                }

                filteredSignal[i] = new ChannelData(1);
                filteredSignal[i].Values[0] = t;
            }

            return filteredSignal;
        }
        #endregion
    }
}
