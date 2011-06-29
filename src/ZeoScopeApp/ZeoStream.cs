namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.IO.Ports;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    enum ZeoDataType
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
    }

    enum ZeoSleepStage
    {
        Undefined0 = 0,
        Awake = 1,
        REM = 2,
        Light = 3,
        Deep = 4,
        Undefined = 5,
    }

    enum ZeoEvent
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

    public class ChannelData
    {
        public ChannelData(int n)
        {
            this.Values = new float[n];
        }

        public float[] Values { get; set; }
    }

    class ZeoMessage
    {
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1);
        public static int SamplesPerMessage = 128;
        public static int FrequencyBinsLength = 7;

        public float[] Waveform;
        public float? Impedance;
        public ZeoEvent? Event;
        public uint ZeoTimestamp;
        public bool? BadSignal;
        public int? SQI;
        public float[] FrequencyBins;
        public ZeoSleepStage? SleepStage;
        public float Second;

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

    public class ZeoStream : IDisposable
    {
        public event EventHandler HeadbandDocked;

        private DateTime unixEpoch = new DateTime(1970, 1, 1);
        public static double SamplesPerSec = 128.0;
        public static int SleepStageSeconds = 30;

        private string fileName;
        private bool isLiveStream;

        private byte[] buffer = new byte[512];
        private List<ZeoMessage> zeoMessages = new List<ZeoMessage>(100);
        private bool writeEnabled = false;

        private SerialPort serialPort;
        private FileStream binFile;

        private Thread readThread;
        private ManualResetEvent exitWorkerEvent;
        private ReaderWriterLock rwLock = new ReaderWriterLock();

        private Stopwatch stopWatch = new Stopwatch();

        public ZeoStream(ManualResetEvent exitWorkerEvent)
        {
            this.exitWorkerEvent = exitWorkerEvent;
        }

        public void OpenLiveStream(string comPortName, string fileName)
        {
            Directory.CreateDirectory("ZeoData");

            this.isLiveStream = true;
            this.fileName = string.Format(@"ZeoData\{0}_{1}.zeo", fileName, DateTime.Now.ToString("yyyy-MM-dd_HHmmss"));
            this.binFile = new FileStream(this.fileName, FileMode.CreateNew);

            this.serialPort = new SerialPort(comPortName, 38400, Parity.None, 8, StopBits.One);
            this.serialPort.Open();
            this.serialPort.DiscardInBuffer();

            this.readThread = new Thread(new ThreadStart(this.ReadSerialStream));
            this.readThread.Start();
        }

        private void ReadSerialStream()
        {
            bool waveMessage = false;

            while (this.exitWorkerEvent.WaitOne(0, true) == false)
            {
                try
                {
                    if (waveMessage == false)
                    {
                        ZeoMessage zeoMessage = ReadMessage();

                        if (zeoMessage.Waveform != null)
                        {
                            waveMessage = true;
                            this.writeEnabled = true;
                        }
                    }

                    if (waveMessage == true)
                    {
                        ZeoMessage zeoMessage = ReadMessage();

                        if (zeoMessage != null)
                        {
                            this.rwLock.AcquireWriterLock(Timeout.Infinite);
                            this.zeoMessages.Add(zeoMessage);
                            this.rwLock.ReleaseLock();

                            if (zeoMessage.Event == ZeoEvent.HeadbandDocked)
                            {
                                if (this.HeadbandDocked != null)
                                {
                                    this.HeadbandDocked(this, null);
                                }

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
        }

        public void OpenFileStream(string fileName)
        {
            this.fileName = fileName;

            this.binFile = new FileStream(this.fileName, FileMode.Open);

            try
            {
                while (true)
                {
                    ZeoMessage zeoMessage = ReadMessage();

                    if (zeoMessage.Waveform != null)
                    {
                        break;
                    }
                }

                while (true)
                {
                    ZeoMessage zeoMessage = ReadMessage();
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

        #region Message Methods
        private ZeoMessage ReadMessage()
        {
            ZeoMessage zeoMessage = new ZeoMessage();

            bool isA4 = false;
            byte header = 0;
            for (int j = 0; j < 20; j++)
            {
                int i;
                for (i = 0; i < 500; i++)
                {
                    if (header == 0x41) // 'A'
                    {
                        header = ReadByte(); ;
                        if (header == 0x34) // '4'
                        {
                            isA4 = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    header = ReadByte();
                }

                if (i != 1)
                {
                    //ZeoSerial.WriteLine("Error; i = {0}", i);
                }

                if (isA4 == true)
                {
                    byte checkSum = ReadByte();

                    ReadNBytes(2);
                    int length = buffer[0] | buffer[1] << 8;

                    ReadNBytes(2);
                    int lengthN = buffer[0] | buffer[1] << 8;

                    if ((short)length != (short)~lengthN)
                    {
                        //ZeoSerial.WriteLine("Error: length = 0x{0:X}, lengthN = 0x{1:X}", length, lengthN);
                        return null;
                    }

                    byte unixT = ReadByte();

                    ReadNBytes(2);
                    float subsecondT = (buffer[0] | buffer[1] << 8) / 65535.0f;

                    byte sequence = ReadByte();

                    ZeoDataType dataType = (ZeoDataType)ReadByte();

                    ReadNBytes(length - 1);

                    switch (dataType)
                    {
                        case ZeoDataType.FrequencyBins:
                            short[] shorts = new short[7];
                            Buffer.BlockCopy(buffer, 0, shorts, 0, 14);
                            float[] floats = new float[7];
                            for (int k = 0; k < 7; k++)
                            {
                                floats[k] = (shorts[k] / 32767.0f) * 100.0f;
                            }

                            zeoMessage.FrequencyBins = floats;
                            break;

                        case ZeoDataType.Waveform:
                            shorts = new short[128];
                            Buffer.BlockCopy(buffer, 0, shorts, 0, 256);

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
                            zeoMessage.ZeoTimestamp = (uint)GetInt32();
                            break;

                        case ZeoDataType.Event:
                            zeoMessage.Event = (ZeoEvent)GetInt32();
                            break;

                        case ZeoDataType.Version:
                            break;

                        case ZeoDataType.SQI:
                            zeoMessage.SQI = GetInt32();
                            break;

                        case ZeoDataType.BadSignal:
                            zeoMessage.BadSignal = GetInt32() == 0 ? false : true;
                            break;

                        case ZeoDataType.Impedance:
                            zeoMessage.Impedance = GetImpedance();
                            break;

                        case ZeoDataType.SleepStage:
                            zeoMessage.SleepStage = (ZeoSleepStage)GetInt32();
                            break;

                        case ZeoDataType.SliceEnd:
                            return zeoMessage;

                        default:
                            return null;
                    }
                }
            }

            return null;
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
                float imp = (float)Math.Sqrt(shorts[0] * shorts[0] + shorts[1] * shorts[1]);
                return float.IsNaN(imp) == false ? imp : 0;
            }
            else
            {
                return 0;
            }
        }

        private void ReadNBytes(int count)
        {
            if (this.isLiveStream == true)
            {
                int n = 0;
                do
                {
                    n += serialPort.Read(this.buffer, n, count - n);
                } while (n < count);

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
            if (this.isLiveStream == true)
            {
                byte b = (byte)serialPort.ReadByte();

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
        #endregion

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        internal ZeoMessage GetMessage(int index)
        {
            this.rwLock.AcquireReaderLock(Timeout.Infinite);

            ZeoMessage zeoMessage = new ZeoMessage();
            if (zeoMessages.Count > index)
            {
                zeoMessage.BadSignal = zeoMessages[index].BadSignal ?? true;
                zeoMessage.Event = zeoMessages[index].Event;
                zeoMessage.FrequencyBins = zeoMessages[index].FrequencyBins;
                zeoMessage.Impedance = zeoMessages[index].Impedance ?? 0;
                zeoMessage.Second = zeoMessages[index].Second;
                zeoMessage.SleepStage = zeoMessages[index].SleepStage;
                zeoMessage.SQI = zeoMessages[index].SQI ?? 0;
                zeoMessage.ZeoTimestamp = zeoMessages[index].ZeoTimestamp;

                if (zeoMessage.SleepStage == null)
                {
                    zeoMessage.SleepStage = ZeoSleepStage.Undefined;
                    for (int i = index - 1; i >= 0; i--)
                    {
                        if (zeoMessages[i].SleepStage != null)
                        {
                            zeoMessage.SleepStage = zeoMessages[i].SleepStage;
                            break;
                        }
                    }
                }
            }

            this.rwLock.ReleaseLock();

            return zeoMessage;
        }

        public ChannelData[] ReadFrequencyDataFromLastPosition(ref int lastPosition, int len)
        {
            this.rwLock.AcquireReaderLock(Timeout.Infinite);

            ChannelData[] freqData = new ChannelData[len];

            for (int i = lastPosition, j = 0; i < (lastPosition + len) && i < zeoMessages.Count; i++, j++)
            {
                ZeoMessage zeoMessage = zeoMessages[i];
                freqData[j] = new ChannelData(ZeoMessage.FrequencyBinsLength + 1);
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
                    freqData[j].Values[freqData[j].Values.Length - 1] = zeoMessage.Impedance ?? 0;
                }
            }

            if (isLiveStream)
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

            for (int i = lastPosition, j = 0; j < len && i < zeoMessages.Count; i++)
            {
                ZeoMessage zeoMessage = zeoMessages[i];
                stageData[j] = new ChannelData(1);
                if (zeoMessage.SleepStage != null)
                {
                    stageData[j].Values[0] = -(int)zeoMessage.SleepStage;
                    j++;
                }
            }

            for (int i = 0; i < len; i++)
            {
                if (stageData[i] == null)
                {
                    stageData[i] = new ChannelData(1);
                    stageData[i].Values[0] = -5;
                }
                else if (stageData[i].Values[0] == 0)
                {
                    stageData[i].Values[0] = -5;
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

            int count = len / ZeoMessage.SamplesPerMessage + 1;
            for (int i = lastPosition, j = 0; i < (lastPosition + count) && i < zeoMessages.Count; i++)
            {
                ZeoMessage zeoMessage = zeoMessages[i];
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

            if (isLiveStream)
            {
                lastPosition = (this.Length * ZeoMessage.SamplesPerMessage - len) / ZeoMessage.SamplesPerMessage;
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

        private ChannelData[] Filter50Hz(ChannelData[] signal)
        {
            float[] filter = {0.0056f, 0.0190f, 0.0113f, -0.0106f, 0.0029f, 0.0041f, 
                            -0.0082f, 0.0089f, -0.0062f, 0.0006f, 0.0066f, -0.0129f,
                            0.0157f, -0.0127f, 0.0035f, 0.0102f, -0.0244f, 0.0336f,
                            -0.0323f, 0.0168f, 0.0136f, -0.0555f, 0.1020f, -0.1446f,
                            0.1743f, 0.8150f, 0.1743f, -0.1446f, 0.1020f, -0.0555f,
                            0.0136f, 0.0168f, -0.0323f, 0.0336f, -0.0244f, 0.0102f,
                            0.0035f, -0.0127f, 0.0157f, -0.0129f, 0.0066f, 0.0006f,
                            -0.0062f, 0.0089f, -0.0082f, 0.0041f, 0.0029f, -0.0106f,
                            0.0113f, 0.0190f, 0.0056f};

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
                    t = t + signal[j].Values[0] * filter[i - j];
                }

                filteredSignal[i] = new ChannelData(1);
                filteredSignal[i].Values[0] = t;
            }

            return filteredSignal;
        }

        public DateTime? GetTimeFromIndex(int index)
        {
            if (zeoMessages.Count > index)
            {
                return this.unixEpoch.AddSeconds(zeoMessages[index].ZeoTimestamp);
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

        public bool LiveStream
        {
            get { return isLiveStream; }
            set { isLiveStream = value; }
        }

        public void Dispose()
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
    }
}
