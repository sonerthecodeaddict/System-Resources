using System;
using System.Runtime.InteropServices;


namespace System_Resources
{

    class Program
    {
        //ram bilgileri için dll
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYCHECK
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYCHECK()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYCHECK));
            }
        }

        //disk bilgileri için dll
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYCHECK lpBuffer);


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
           out ulong lpFreeBytesAvailable,
           out ulong lpTotalNumberOfBytes,
           out ulong lpTotalNumberOfFreeBytes);


        //arkaplan resmi değiştirmek için dll
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);


        //Çözünürlük işlemleri  için dll
        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int DISP_CHANGE_SUCCESSFUL = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;

            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;

            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;

            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }

        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);



        static void Main()
        {
            DiskInfo();
            RamInfo();
            SetBackGround();
            DisplayOperations();
        }


        public static void DiskInfo()
        {
            string drive = "C:\\";
            ulong freeBytesAvailable, totalNumberOfBytes, totalNumberOfFreeBytes;

            bool diskSuccess = GetDiskFreeSpaceEx(drive, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);

            if (diskSuccess)
            {
                Console.WriteLine($"sürücü: {drive}");
                Console.WriteLine($"Toplam alan: {totalNumberOfBytes} byte");
                Console.WriteLine($"boş alan: {totalNumberOfFreeBytes} byte");
                Console.WriteLine($"kullanılabilir alan: {freeBytesAvailable} byte");
            }

            else
            {
                Console.WriteLine("hata");
            }
        }


        public static void RamInfo()
        {
            Console.WriteLine("\nRam:");

            MEMORYCHECK memStatus = new MEMORYCHECK();

            if (GlobalMemoryStatusEx(memStatus))
            {
                Console.WriteLine($"toplam bellek: {memStatus.ullTotalPhys / 1024 / 1024} MB");
                Console.WriteLine($"kullanılabilir bellek: {memStatus.ullAvailPhys / 1024 / 1024} MB");
                Console.WriteLine($"toplam sanal bellek: {memStatus.ullTotalVirtual / 1024 / 1024 / 1024} GB");
                Console.WriteLine($"kullanılabilir sanal bellek: {memStatus.ullAvailVirtual / 1024 / 1024 / 1024} GB");
            }

            else
            {
                Console.WriteLine("hata");
            }
        }


        public static void SetBackGround()
        {
            string wallpaperPath = @"C:\Users\Soner Koçak\Desktop\Görseller\12-123231_rick-and-morty-wallpaper-rick-and-morty-wallpaper.jpg";
            bool result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, wallpaperPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE) != 0;

            if (result)
            {
                Console.WriteLine("\nArka plan degistirdi");
            }

            else
            {
                Console.WriteLine("\nhata");
            }
        }

        public static void DisplayOperations()
        {
            //mevcut çözünürlük
            DEVMODE devMode = new DEVMODE();
            devMode.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE));

            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref devMode) != 0)
            {
                Console.WriteLine($"\nMevcut çözünürlük: {devMode.dmPelsWidth}x{devMode.dmPelsHeight}");
            }

            else
            {
                Console.WriteLine("\nhata");
            }

            //çözünürlüğü değiştirme
            devMode.dmPelsWidth = 1920;
            devMode.dmPelsHeight = 1080;
            int result = ChangeDisplaySettings(ref devMode, 0);

            if (result == DISP_CHANGE_SUCCESSFUL)
            {
                Console.WriteLine("çözünürlük değişti");
            }
            else
            {
                Console.WriteLine("hata");
            }

        }

    }
}