using Mono.Unix.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Vibrometer.Testing
{
    class Program
    {
        const int CLOCK_RATE            = 125000000;
        const int GPIO_REG_COUNT        = 5;
        const int GPIO_REG_SIZE         = 65536;
        const int GPIO_BASE             = 0x4120_0000;
        const int GPIO_GENERAL          = 0x0000_0000;
        const int GPIO_SIGNAL_GENERATOR = 0x0001_0000;
        const int GPIO_DATA_ACQUISITION = 0x0002_0000;
        const int GPIO_POSITION_TRACKER = 0x0003_0000;
        const int GPIO_RAM_WRITER       = 0x0004_0000;

        static uint _gpio_general;
        static uint _gpio_signal_generator;
        static uint _gpio_data_acquisition;
        static uint _gpio_position_tracker;
        static uint _gpio_ram_writer;

        static IntPtr _GPIO;
        static IntPtr _read_buffer;

        static void Main(string[] args)
        {
            Program.MemoryMap();

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"GPIO memory map: 0x{(uint)_GPIO,8:X}");
                Console.WriteLine($"Read buffer:     0x{(uint)_read_buffer,8:X}");
                Console.WriteLine("[L] - Load FPGA image");
                Console.WriteLine("[0] - GE_Get_Position");
                Console.WriteLine("[1] - SG_Set_Phase");
                Console.WriteLine("[2] - DA_Set_SwitchEnable");
                Console.WriteLine("[3] - DA_Set_SwitchDisable");
                Console.WriteLine("[4] - DA_Get_Raw");
                Console.WriteLine("[5] - PT_Set_LogCountExtremum");
                Console.WriteLine("[6] - PT_Set_ShiftExtremum");
                Console.WriteLine("[7] - PT_Get_Threshold");
                Console.WriteLine("[8] - RW_Set_LogLength");
                Console.WriteLine("[9] - RW_Set_RequestEnable");
                Console.WriteLine("[A] - RW_Set_RequestDisable");
                Console.WriteLine("[B] - RW_Set_Address");
                Console.WriteLine("[C] - RW_Get_ReadBuffer");
                Console.WriteLine();

                var keyInfo = Console.ReadKey();

                switch(keyInfo.Key)
                {
                    case ConsoleKey.L:
                        Program.LoadFPGAImage();
                        break;
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.D0:
                        Program.GE_Get_Position();
                        break;
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        Program.SG_Set_Phase();
                        break;
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        Program.DA_Set_SwitchEnable();
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        Program.DA_Set_SwitchDisable();
                        break;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        Program.DA_Get_Raw();
                        break;
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        Program.PT_Set_LogCountExtremum();
                        break;
                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        Program.PT_Set_ShiftExtremum();
                        break;
                    case ConsoleKey.NumPad7:
                    case ConsoleKey.D7:
                        Program.PT_Get_Threshold();
                        break;
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        Program.RW_Set_LogLength();
                        break;
                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        Program.RW_Set_RequestEnable();
                        break;
                    case ConsoleKey.A:
                        Program.RW_Set_RequestDisable();
                        break;
                    case ConsoleKey.B:
                        Program.RW_Set_Address();
                        break;
                    case ConsoleKey.C:
                        Program.RW_Get_ReadBuffer();
                        break;
                    default:
                        break;
                }
            }

            Syscall.munmap(_GPIO, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE));
            // TODO: free AlloHGlobal
        }

        private static void MemoryMap()
        {
            var fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);
            _GPIO = Syscall.mmap(IntPtr.Zero, GPIO_REG_COUNT * GPIO_REG_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, GPIO_BASE);
        }

        private static void LoadFPGAImage()
        {
            Console.Clear();
            Console.WriteLine("Please enter the file path of FPGA image:");
            Console.WriteLine();

            var filePath = Console.ReadLine();

            if (File.Exists(filePath))
            {
                using (var sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var targetFileStream = File.Open("/dev/xdevcfg", FileMode.Open, FileAccess.Write))
                    {
                        sourceFileStream.CopyTo(targetFileStream);
                    }
                }
            }
        }

        // API

        private static void GE_Get_Position()
        {
            Task task;
            int position;
            CancellationTokenSource cts;

            cts = new CancellationTokenSource();

            Console.Clear();

            task = Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    unsafe
                    {
                        position = *(int*)(_GPIO + GPIO_GENERAL);
                    }

                    Console.WriteLine($"{position,8:X}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void SG_Set_Phase()
        {
            double max_value;
            double value;

            unsafe
            {
                _gpio_signal_generator = *(uint*)(_GPIO + GPIO_SIGNAL_GENERATOR);
            }

            max_value = CLOCK_RATE;
            value = _gpio_signal_generator / Math.Pow(2, 28) * CLOCK_RATE;

            Console.Clear();
            Console.WriteLine($"Current: {value:F2} Hz, max: 0 <= value <= { max_value } Hz");
            Console.WriteLine($"Please enter the desired signal generator frequency:");
            Console.WriteLine();

            while (!double.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _gpio_signal_generator = (uint)(value * Math.Pow(2, 28) / CLOCK_RATE);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_SIGNAL_GENERATOR) = _gpio_signal_generator;
            }
        }

        private static void DA_Set_SwitchEnable()
        {
            _gpio_data_acquisition |= (1U << 0);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_DATA_ACQUISITION) = _gpio_data_acquisition;
            }
        }

        private static void DA_Set_SwitchDisable()
        {
            _gpio_data_acquisition &= ~(1U << 0);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_DATA_ACQUISITION) = _gpio_data_acquisition;
            }
        }

        private static void DA_Get_Raw()
        {
            uint raw;
            short a;
            short b;
            Task task;
            CancellationTokenSource cts;

            cts = new CancellationTokenSource();

            Console.Clear();

            task = Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    unsafe
                    {
                        raw = *(uint*)(_GPIO + GPIO_DATA_ACQUISITION + 0x08);
                    }

                    a = unchecked((short)(raw & ~0xFFFF0000));
                    b = unchecked((short)(raw >> 16));

                    Console.WriteLine($"hex: {raw,8:X}, a: {a,10}, b: {b,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void PT_Set_LogCountExtremum()
        {
            uint value;
            uint max_value;

            unsafe
            {
                _gpio_position_tracker = *(uint*)(_GPIO + GPIO_POSITION_TRACKER);
            }

            max_value = (uint)Math.Pow(2, 5) - 1;
            value = (_gpio_position_tracker >> 3) & max_value;

            Console.Clear();
            Console.WriteLine($"Current: { value }, max: (0 <= value <= { max_value })");
            Console.WriteLine($"Please enter the desired log count extremum:");
            Console.WriteLine();

            while (!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _gpio_position_tracker &= ~(max_value << 3);
            _gpio_position_tracker |= (value << 3);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_POSITION_TRACKER) = _gpio_position_tracker;
            }
        }

        private static void PT_Set_ShiftExtremum()
        {
            uint value;
            uint max_value;

            unsafe
            {
                _gpio_position_tracker = *(uint*)(_GPIO + GPIO_POSITION_TRACKER);
            }

            max_value = (uint)Math.Pow(2, 3) - 1;
            value = (_gpio_position_tracker >> 0) & max_value;

            Console.Clear();
            Console.WriteLine($"Current: { value }, max: (0 <= value <= { max_value })");
            Console.WriteLine($"Please enter the desired shift extremum:");
            Console.WriteLine();

            while (!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _gpio_position_tracker &= ~(max_value << 0);
            _gpio_position_tracker |= (value << 0);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_POSITION_TRACKER) = _gpio_position_tracker;
            }
        }

        private static void PT_Get_Threshold()
        {
            uint raw;
            short a;
            short b;
            Task task;
            CancellationTokenSource cts;

            cts = new CancellationTokenSource();

            Console.Clear();

            task = Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    unsafe
                    {
                        raw = *(uint*)(_GPIO + GPIO_POSITION_TRACKER + 0x08);
                    }

                    a = unchecked((short)(raw & ~0xFFFF0000));
                    b = unchecked((short)(raw >> 16));

                    Console.WriteLine($"hex: {raw,8:X}, high: {b,10}, low: {a,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void RW_Set_LogLength()
        {
            uint value;
            uint max_value;

            unsafe
            {
                _gpio_ram_writer = *(uint*)(_GPIO + GPIO_RAM_WRITER);
            }

            max_value = (uint)Math.Pow(2, 5) - 1;
            value = (_gpio_ram_writer >> 1) & max_value;

            Console.Clear();
            Console.WriteLine($"Current: { value }, max: (0 <= value <= { max_value })");
            Console.WriteLine($"Please enter the desired log length:");
            Console.WriteLine();

            while (!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _gpio_ram_writer &= ~(max_value << 1);
            _gpio_ram_writer |= (value << 1);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_RAM_WRITER) = _gpio_ram_writer;
            }
        }

        private static void RW_Set_RequestEnable()
        {
            _gpio_ram_writer |= (1U << 0);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_RAM_WRITER) = _gpio_ram_writer;
            }
        }

        private static void RW_Set_RequestDisable()
        {
            _gpio_ram_writer &= ~(1U << 0);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_RAM_WRITER) = _gpio_ram_writer;
            }
        }

        private static void RW_Set_Address()
        {
            _read_buffer = Marshal.AllocHGlobal(1024);

            unsafe
            {
                *(uint*)(_GPIO + GPIO_RAM_WRITER + 0x08) = (uint)_read_buffer.ToPointer();
            }
        }

        private static void RW_Get_ReadBuffer()
        {
            uint address;
            uint data;

            Console.Clear();

            unsafe
            {
                address = *(uint*)(_GPIO + GPIO_RAM_WRITER + 0x08);
            }

            Console.WriteLine($"0x{address,8:X}");
            Console.WriteLine();

            unsafe
            {
                data = *(uint*)(address + 0x0000);
                Console.WriteLine($"{ data }");
                data = *(uint*)(address + 0x0001);
                Console.WriteLine($"{ data }");
                data = *(uint*)(address + 0x0010);
                Console.WriteLine($"{ data }");
                data = *(uint*)(address + 0x0011);
                Console.WriteLine($"{ data }");
                data = *(uint*)(address + 0x0100);
                Console.WriteLine($"{ data }");
                data = *(uint*)(address + 0x0101);
                Console.WriteLine($"{ data }");
            }

            Console.ReadKey(true);
        }     
    }
}
