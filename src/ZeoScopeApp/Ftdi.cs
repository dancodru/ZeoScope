namespace ZeoScope
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;

    enum FTStatusCode : uint
    {
        FT_OK,
        FT_INVALID_HANDLE,
        FT_DEVICE_NOT_FOUND,
        FT_DEVICE_NOT_OPENED,
        FT_IO_ERROR,
        FT_INSUFFICIENT_RESOURCES,
        FT_INVALID_PARAMETER,
        FT_INVALID_BAUD_RATE,

        FT_DEVICE_NOT_OPENED_FOR_ERASE,
        FT_DEVICE_NOT_OPENED_FOR_WRITE,
        FT_FAILED_TO_WRITE_DEVICE,
        FT_EEPROM_READ_FAILED,
        FT_EEPROM_WRITE_FAILED,
        FT_EEPROM_ERASE_FAILED,
        FT_EEPROM_NOT_PRESENT,
        FT_EEPROM_NOT_PROGRAMMED,
        FT_INVALID_ARGS,
        FT_NOT_SUPPORTED,
        FT_OTHER_ERROR,
        FT_DEVICE_LIST_NOT_READY,
    }

    enum FtDevice
    {
        FT_DEVICE_BM,
        FT_DEVICE_AM,
        FT_DEVICE_100AX,
        FT_DEVICE_UNKNOWN,
        FT_DEVICE_2232,
        FT_DEVICE_232R,
        FT_DEVICE_2232H,
        FT_DEVICE_4232H
    }

    struct FTDeviceInfo
    {
        public uint Flags;
        public FtDevice Type;
        public uint ID;
        public uint LocId;
        public string SerialNumber;
        public string Description;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct FTDeviceInfoNode
    {
        public uint Flags;
        public FtDevice Type;
        public uint ID;
        public uint LocId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] SerialNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Description;
        public IntPtr ftHandle;
    }

    enum FTOpenByFlag : uint
    {
        FT_OPEN_BY_SERIAL_NUMBER = 1,
        FT_OPEN_BY_DESCRIPTION = 2,
        FT_OPEN_BY_LOCATION = 4
    }

    class Ftdi
    {
        [DllImport("ftd2xx.dll", ExactSpelling = true)]
        private static extern FTStatusCode FT_CreateDeviceInfoList(
            [Out] out uint lpdwNumDevs);

        [DllImport("ftd2xx.dll", ExactSpelling = true)]
        private static extern FTStatusCode FT_GetDeviceInfoList(
            [In, Out] FTDeviceInfoNode[] pDest,
            [In, Out] ref uint lpdwNumDevs);

        [DllImport("ftd2xx.dll", ExactSpelling = true)]
        private static extern FTStatusCode FT_OpenEx(string devstring, uint dwFlags, ref IntPtr ftHandle);

        [DllImport("ftd2xx.dll", ExactSpelling = true)]
        private static extern FTStatusCode FT_Close(IntPtr ftHandle);

        [DllImport("ftd2xx.dll", ExactSpelling = true)]
        private static extern FTStatusCode FT_GetComPortNumber(IntPtr ftHandle, out int dwComPortNumber);

        public static string[] GetComPortList()
        {
            List<string> comPorts = new List<string>();
            try
            {
                List<FTDeviceInfo> ftdiDevices = new List<FTDeviceInfo>();

                uint deviceNumber;
                FTStatusCode ftStatus = Ftdi.FT_CreateDeviceInfoList(out deviceNumber);

                if (deviceNumber > 0)
                {
                    FTDeviceInfoNode[] devices = new FTDeviceInfoNode[deviceNumber];

                    for (int k = 0; k < deviceNumber; k++)
                    {
                        devices[k].SerialNumber = new byte[16];
                        devices[k].Description = new byte[64];
                    }

                    ftStatus = Ftdi.FT_GetDeviceInfoList(devices, ref deviceNumber);

                    for (int k = 0; k < deviceNumber; k++)
                    {
                        FTDeviceInfo ftDeviceInfo = new FTDeviceInfo();
                        ftDeviceInfo.Flags = devices[k].Flags;
                        ftDeviceInfo.Type = devices[k].Type;
                        ftDeviceInfo.ID = devices[k].ID;
                        ftDeviceInfo.LocId = devices[k].LocId;

                        ftDeviceInfo.Description = ASCIIEncoding.ASCII.GetString(devices[k].Description);
                        ftDeviceInfo.Description = ftDeviceInfo.Description.Substring(0, ftDeviceInfo.Description.IndexOf("\0"));
                        ftDeviceInfo.SerialNumber = ASCIIEncoding.ASCII.GetString(devices[k].SerialNumber);
                        ftDeviceInfo.SerialNumber = ftDeviceInfo.SerialNumber.Substring(0, ftDeviceInfo.SerialNumber.IndexOf("\0"));

                        ftdiDevices.Add(ftDeviceInfo);
                    }

                    foreach (FTDeviceInfo ftdiDevice in ftdiDevices)
                    {
                        string comPort = Ftdi.GetComPortNumber(ftdiDevice.SerialNumber);
                        if (string.IsNullOrEmpty(comPort) == false)
                        {
                            comPorts.Add(comPort);
                        }
                    }
                }

            }
            catch (DllNotFoundException)
            {
            }

            comPorts.Sort();
            return comPorts.ToArray();
        }

        private static string GetComPortNumber(string serialNumber)
        {
            IntPtr ftHandle = IntPtr.Zero; 
            FTStatusCode ftStatus = Ftdi.FT_OpenEx(serialNumber, (uint)FTOpenByFlag.FT_OPEN_BY_SERIAL_NUMBER, ref ftHandle);

            string comNumber = null;
            ftStatus = FTStatusCode.FT_OTHER_ERROR;

            if (ftHandle != IntPtr.Zero)
            {
                int portNumber;
                ftStatus = Ftdi.FT_GetComPortNumber(ftHandle, out portNumber);
                if (ftStatus == FTStatusCode.FT_OK && portNumber > 0)
                {
                    comNumber = "COM" + portNumber.ToString();
                }

                ftStatus = Ftdi.FT_Close(ftHandle);
            }

            return comNumber;
        }
    }
}
