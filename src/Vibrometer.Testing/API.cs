using Mono.Unix.Native;
using System;
using System.IO;

namespace Vibrometer.Testing
{
    public static class API
    {
        public const int CLOCK_RATE = 125000000;
        public const int DATA_BASE = 0x1E00_0000;

        private const int GPIO_BASE = 0x4120_0000;
        private const int GPIO_REG_COUNT = 5;
        private const int GPIO_REG_SIZE = 0x0001_0000;
        private const int GPIO_SIZE = GPIO_REG_COUNT * GPIO_REG_SIZE;
        private const int GPIO_GENERAL = 0x0000_0000;
        private const int GPIO_SIGNAL_GENERATOR = 0x0001_0000;
        private const int GPIO_DATA_ACQUISITION = 0x0002_0000;
        private const int GPIO_POSITION_TRACKER = 0x0003_0000;
        private const int GPIO_RAM_WRITER = 0x0004_0000;

        private const int DATA_SIZE = 0x0100_0000;

        private static IntPtr _GPIO;
        private static IntPtr _DATA;

        // helper
        private static void SetValue(int width, int shift, IntPtr address, uint value)
        {
            uint storage;
            uint max;

            max = (uint)Math.Pow(2, width) - 1;

            if (value > max)
            {
                throw new ArgumentException(nameof(value));
            }

            storage = API.GetValue(32, 0, address);
            storage &= ~(max << shift);
            storage |= (value << shift);

            unsafe
            {
                *(uint*)(address.ToPointer()) = storage;
            }
        }

        private static uint GetValue(int width, int shift, IntPtr address)
        {
            uint value;
            uint max;

            max = (uint)(Math.Pow(2, width) - 1);

            unsafe
            {
                value = *(uint*)(address.ToPointer());
            }

            return (value >> shift) & max;
        }

        // API (high level)
        public static void Init()
        {
            int fd;

            fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);

            _GPIO = Syscall.mmap(IntPtr.Zero, GPIO_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, GPIO_BASE);
            _DATA = Syscall.mmap(IntPtr.Zero, DATA_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, DATA_BASE);

            Syscall.close(fd);
        }

        public static void Free()
        {
            if (_GPIO.ToInt64() > 0)
            {
                Syscall.munmap(_GPIO, GPIO_SIZE);
            }

            if (_DATA.ToInt64() > 0)
            {
                Syscall.munmap(_DATA, DATA_SIZE);
            }
        }

        public static void SetDefaults()
        {
            // 100 Hz
            API.SignalGenerator.Phase = 100;

            // approx. 1s
            API.PositionTracker.LogCountExtremum = 27;

            // central value +- max / (2^4)
            API.PositionTracker.ShiftExtremum = 4;

            // physical RAM address
            API.RamWriter.Address = DATA_BASE;

            // buffer length = 2^12 = 4096 = 1024 * 4 byte
            API.RamWriter.LogLength = 12;
            
            // throttle 125 MHz ADC data by factor 2^12 = 4096 to get into the kHz range
            API.RamWriter.LogThrottle = 12;

            // enable RAM writer
            API.RamWriter.Enabled = true;
        }

        public static Span<int> GetBuffer(int offset)
        {
            int length;

            length = (int)Math.Min(Math.Pow(2, API.RamWriter.LogLength) / 4, 1024);

            unsafe
            {
                return new Span<int>((int*)IntPtr.Add(_DATA, offset), length);
            }
        }

        public static void Clear()
        {
            bool enabled;
            Span<byte> data;

            enabled = API.RamWriter.Enabled;
            API.RamWriter.Enabled = false;

            unsafe
            {
                data = new Span<byte>((byte*)_DATA, (int)Math.Pow(2, API.RamWriter.LogLength));
            }

            data.Clear();
            API.RamWriter.Enabled = enabled;
        }

        public static void LoadFPGAImage(string filePath)
        {
            using (var sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var targetFileStream = File.Open("/dev/xdevcfg", FileMode.Open, FileAccess.Write))
                {
                    sourceFileStream.CopyTo(targetFileStream);
                }
            }
        }

        // API (low level)
        public static class General
        {
            public static int Position
            {
                get
                {
                    return (int)API.GetValue(32, 0, _GPIO + GPIO_GENERAL);
                }
            }
        }

        public static class SignalGenerator
        {
            public static uint Phase
            {
                get
                {
                    return API.GetValue(32, 0, _GPIO + GPIO_SIGNAL_GENERATOR);
                }
                set
                {
                    if (value > CLOCK_RATE)
                    {
                        throw new ArgumentException(nameof(value));
                    }

                    API.SetValue(32, 0, _GPIO + GPIO_SIGNAL_GENERATOR, (uint)(value * Math.Pow(2, 28) / CLOCK_RATE));
                }
            }
        }

        public static class DataAcquisition
        {
            public static bool SwitchEnabled
            {
                get
                {
                    return API.GetValue(1, 0, _GPIO + GPIO_DATA_ACQUISITION) > 0;
                }
                set
                {
                    API.SetValue(1, 0, _GPIO + GPIO_DATA_ACQUISITION, value ? 1U : 0U);
                }
            }

            public static (short, short) Raw
            {
                get
                {
                    short a;
                    short b;
                    uint value;

                    value = API.GetValue(32, 0, _GPIO + GPIO_DATA_ACQUISITION + 0x08);

                    a = unchecked((short)(value & ~0xFFFF0000));
                    b = unchecked((short)(value >> 16));

                    return (a, b);
                }
            }
        }

        public static class PositionTracker
        {
            public static uint LogCountExtremum
            {
                get
                {
                    return API.GetValue(5, 3, _GPIO + GPIO_POSITION_TRACKER);
                }
                set
                {
                    API.SetValue(5, 3, _GPIO + GPIO_POSITION_TRACKER, value);
                }
            }

            public static uint ShiftExtremum
            {
                get
                {
                    return API.GetValue(3, 0, _GPIO + GPIO_POSITION_TRACKER);
                }
                set
                {
                    API.SetValue(3, 0, _GPIO + GPIO_POSITION_TRACKER, value);
                }
            }

            public static (short, short) Threshold
            {
                get
                {
                    short a;
                    short b;
                    uint value;

                    value = API.GetValue(32, 0, _GPIO + GPIO_POSITION_TRACKER + 0x08);

                    a = unchecked((short)(value & ~0xFFFF0000));
                    b = unchecked((short)(value >> 16));

                    return (a, b);
                }
            }
        }

        public static class RamWriter
        {
            public static bool Enabled
            {
                get
                {
                    return API.GetValue(1, 0, _GPIO + GPIO_RAM_WRITER) > 0;
                }
                set
                {
                    API.SetValue(1, 0, _GPIO + GPIO_RAM_WRITER, value ? 1U : 0U);
                }
            }

            public static bool RequestEnabled
            {
                get
                {
                    return API.GetValue(1, 1, _GPIO + GPIO_RAM_WRITER) > 0;
                }
                set
                {
                    API.SetValue(1, 1, _GPIO + GPIO_RAM_WRITER, value ? 1U : 0U);
                }
            }

            public static uint LogLength
            {
                get
                {
                    return API.GetValue(5, 2, _GPIO + GPIO_RAM_WRITER);
                }
                set
                {
                    API.SetValue(5, 2, _GPIO + GPIO_RAM_WRITER, value);
                }
            }

            public static uint LogThrottle
            {
                get
                {
                    return API.GetValue(5, 7, _GPIO + GPIO_RAM_WRITER);
                }
                set
                {
                    if (value < 1)
                    {
                        throw new ArgumentException(nameof(value));
                    }

                    API.SetValue(5, 7, _GPIO + GPIO_RAM_WRITER, value);
                }
            }

            public static uint Address
            {
                set
                {
                    API.SetValue(32, 0, _GPIO + GPIO_RAM_WRITER + 0x08, value);
                }
            }

            public static uint ReadBuffer
            {
                get
                {
                    return API.GetValue(32, 0, _GPIO + GPIO_RAM_WRITER + 0x08);
                }
            }
        }

        public static class Ram
        {
            public static int GetData(int offset)
            {
                unsafe
                {
                    return *(int*)(IntPtr.Add(_DATA, offset));
                }
            }
        }
    }
}
