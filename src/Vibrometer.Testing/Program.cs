using Mono.Unix.Native;
using System;
using System.IO;
using System.Threading;

namespace Vibrometer.Testing
{
    class Program
    {
        /*
        -> axi_gpio_0 (0x4120_0000, GPIO_pt):
	        [8:8]  - enable	
	        [7:3]  - log_count_extremum
	        [2:0]  - shift_extremum

        -> axi_gpio_0 (0x4120_0008, GPIO_sg):
	        [31:0] - phase


        -> axi_gpio_1 (0x4121_0000, GPIO_rw_1):
	        [5:1]  - log_length
	        [0:0]  - request

        -> axi_gpio_1 (0x4121_0008, GPIO_rw_2):
	        [31:0] - address

        <- axi_gpio_1 (GPIO_rw_1):
	        [31:0] - read_buffer  
        */

        static uint _GPIO_0_0_value;
        static uint _GPIO_0_1_value;
        static uint _GPIO_1_0_value;
        static uint _GPIO_1_1_value;

        static IntPtr _GPIO_0;
        static IntPtr _GPIO_1;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"GPIO 0: { (uint)_GPIO_0 }, GPIO 1: { (uint)_GPIO_1 }");
                Console.WriteLine("[Tab] - Memory Map");
                Console.WriteLine("[0] - Load FPGA image");
                Console.WriteLine("[1] - Set_Enable");
                Console.WriteLine("[2] - Set_Disable");
                Console.WriteLine("[3] - PT_Set_LogCountExtremum");
                Console.WriteLine("[4] - PT_Set_LogShiftExtremum");
                Console.WriteLine("[5] - SG_Set_Phase");
                Console.WriteLine("[6] - RW_Set_LogLength");
                Console.WriteLine("[7] - RW_Set_RequestEnable");
                Console.WriteLine("[8] - RW_Set_RequestDisable");
                Console.WriteLine("[9] - RW_Set_Address");
                Console.WriteLine("[a] - RW_Get_ReadBuffer");
                Console.WriteLine();

                var keyInfo = Console.ReadKey();

                switch(keyInfo.Key)
                {
                    case ConsoleKey.Tab:
                        Program.MemoryMap();
                        break;
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.D0:
                        Program.LoadFPGAImage();
                        break;
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        Program.Set_Enable();
                        break;
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        Program.Set_Disable();
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        Program.PT_Set_LogCountExtremum();
                        break;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        Program.PT_Set_ShiftExtremum();
                        break;
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        Program.SG_Set_Phase();
                        break;
                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        Program.RW_Set_LogLength();
                        break;
                    case ConsoleKey.NumPad7:
                    case ConsoleKey.D7:
                        Program.RW_Set_RequestEnable();
                        break;
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        Program.RW_Set_RequestDisable();
                        break;
                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        Program.RW_Set_Address();
                        break;
                    case ConsoleKey.A:
                        Program.RW_Get_ReadBuffer();
                        break;
                    default:
                        break;
                }
            }

            Syscall.munmap(_GPIO_0, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE));
            Syscall.munmap(_GPIO_1, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE));
        }

        private static void MemoryMap()
        {
            var fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);

            _GPIO_0 = Syscall.mmap(IntPtr.Zero, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE), MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, 0x4120_0000);
            _GPIO_1 = Syscall.mmap(IntPtr.Zero, (ulong)Syscall.sysconf(SysconfName._SC_PAGESIZE), MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, 0x4121_0000);
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

        private static void Set_Enable()
        {
            _GPIO_0_0_value |= (1U << 8);

            unsafe
            {
                *(uint*)(_GPIO_0 + 0) = _GPIO_0_0_value;
            }
        }

        private static void Set_Disable()
        {
            _GPIO_0_0_value &= ~(1U << 8);

            unsafe
            {
                *(uint*)(_GPIO_0 + 0) = _GPIO_0_0_value;
            }
        }

        private static void PT_Set_LogCountExtremum()
        {
            uint max_value = (uint)Math.Pow(2, 5) - 1;

            Console.Clear();
            Console.WriteLine($"Please enter the desired log count extremum (0 <= value <= { max_value }):");
            Console.WriteLine();

            uint value = 0;

            while (!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _GPIO_0_0_value &= ~(max_value << 3);
            _GPIO_0_0_value |= (value << 3);

            unsafe
            {
                *(uint*)(_GPIO_0 + 0) = _GPIO_0_0_value;
            }
        }

        private static void PT_Set_ShiftExtremum()
        {
            uint max_value = (uint)Math.Pow(2, 3) - 1;

            Console.Clear();
            Console.WriteLine($"Please enter the desired shift extremum (0 <= value <= { max_value }):");
            Console.WriteLine();

            uint value = 0;

            while (!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _GPIO_0_0_value &= ~(max_value << 0);
            _GPIO_0_0_value |= (value << 0);

            unsafe
            {
                *(uint*)(_GPIO_0 + 0) = _GPIO_0_0_value;
            }
        }

        private static void SG_Set_Phase()
        {
            uint max_value = (uint)Math.Pow(2, 28) - 1;

            Console.Clear();
            Console.WriteLine($"Please enter the desired signal generator frequency: (0 <= value <= { max_value }):");
            Console.WriteLine();

            uint value = 0;
                
            while(!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            uint phase = (uint)(value * 2^28 / (125000000));

            _GPIO_0_1_value = phase;

            unsafe
            {
                *(uint*)(_GPIO_0 + 8) = _GPIO_0_1_value;
            }
        }

        private static void RW_Set_LogLength()
        {
            uint max_value = (uint)Math.Pow(2, 5) - 1;

            Console.Clear();
            Console.WriteLine($"Please enter the desired log shift extremum (0 <= value <= { max_value }):");
            Console.WriteLine();

            uint value = 0;

            while (!uint.TryParse(Console.ReadLine(), out value) || value > max_value)
            {
                //
            }

            _GPIO_1_0_value &= ~(max_value << 1);
            _GPIO_1_0_value |= (value << 1);

            unsafe
            {
                *(uint*)(_GPIO_1 + 0) = _GPIO_1_0_value;
            }
        }

        private static void RW_Set_RequestEnable()
        {
            _GPIO_1_0_value |= (1U << 0);

            unsafe
            {
                *(uint*)(_GPIO_1 + 0) = _GPIO_1_0_value;
            }
        }

        private static void RW_Set_RequestDisable()
        {
            _GPIO_1_0_value &= ~(1U << 0);

            unsafe
            {
                *(uint*)(_GPIO_1 + 0) = _GPIO_1_0_value;
            }
        }

        private static void RW_Set_Address()
        {
            throw new NotImplementedException();
        }

        private static void RW_Get_ReadBuffer()
        {
            uint address;
            int address1;
            int address2;

            unsafe
            {
                address1 = *(int*)(_GPIO_1 + 0);
            }

            Console.Clear();
            Console.WriteLine($"Current read buffer address is: { address1 } | { (short)((address1 & 0xFFFF0000) >> 16) } | { (short)(address1 & 0x0000FFFF) }, HEX: {((address1 & 0xFFFF0000) >> 16):X} | {(address1 & 0x0000FFFF):X}");
            Console.ReadKey(true);

            while (true)
            {
                unsafe
                {
                    address = *(uint*)(_GPIO_1 + 0);
                }
                address2 = (int)((address << 2) & ~0xFFFF0000);
                address2 ^= (1 << 15); 
                address1 = (int)(address >> 15);
                Console.WriteLine($"hex: {address,8:X}, high: { ~address1,10 }, low: { ~address2, 10 }");
                Thread.Sleep(50);
            }
        }
    }
}
