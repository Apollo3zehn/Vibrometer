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
        //[DllImport("c", CharSet = CharSet.Auto)]
        //static unsafe extern void* dma_alloc_coherent(void* dev, uint size, uint* dma_addr, uint flag);

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //Console.ReadKey(true);

            //unsafe
            //{
            //    uint yellow;
            //    var res = dma_alloc_coherent(null, 10, &yellow, 0);
            //    uint a = *(uint*)res;

            //}

            //return;
            bool exit = false;

            API.Init();

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine($"[L] - load FPGA image");
                Console.WriteLine($"[P] - set defaults");

                Console.WriteLine();

                Console.WriteLine("General");
                Console.WriteLine($"[0] - get position");

                Console.WriteLine();

                Console.WriteLine("Signal Generator");
                Console.WriteLine($"[1] - set phase");

                Console.WriteLine();

                Console.WriteLine("Data Acquisition");

                if (API.DataAcquisition.SwitchEnabled)
                    Program.WriteColored($"[2] - disable switch\n");
                else
                    Console.Write($"[2] - enable switch\n");

                Console.WriteLine($"[3] - get raw");

                Console.WriteLine();

                Console.WriteLine("Position Tracker");
                Console.WriteLine($"[4] - set log count extremum");
                Console.WriteLine($"[5] - set shift extremum");
                Console.WriteLine($"[6] - get threshold");

                Console.WriteLine();

                Console.WriteLine("RAM Writer");

                if (API.RamWriter.Enabled)
                    Program.WriteColored($"[7] - disable RAM writer\n");
                else
                    Console.Write($"[7] - enable RAM writer\n");

                if (API.RamWriter.RequestEnabled)
                    Program.WriteColored($"[8] - disable buffer request\n");
                else
                    Console.Write($"[8] - enable buffer request\n");

                Console.WriteLine($"[9] - set log length");
                Console.WriteLine($"[A] - set log throttle");
                Console.WriteLine($"[B] - set address");
                Console.WriteLine($"[C] - get read buffer");

                Console.WriteLine();

                Console.WriteLine("RAM");
                Console.WriteLine($"[D] - get data ({Math.Min(Math.Pow(2, API.RamWriter.LogLength), 1024)} values)");
                Console.WriteLine($"[E] - get stream");
                Console.WriteLine($"[F] - clear");

                var keyInfo = Console.ReadKey(true);

                switch(keyInfo.Key)
                {
                    case ConsoleKey.L:
                        Program.LoadFPGAImage();
                        break;
                    case ConsoleKey.P:
                        API.SetDefaults();
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
                        Program.DA_Toggle_Switch();
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        Program.DA_Get_Raw();
                        break;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        Program.PT_Set_LogCountExtremum();
                        break;
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        Program.PT_Set_ShiftExtremum();
                        break;
                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        Program.PT_Get_Threshold();
                        break;
                    case ConsoleKey.NumPad7:
                    case ConsoleKey.D7:
                        Program.RW_Toggle_Enable();
                        break;
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        Program.RW_Toggle_RequestEnable();
                        break;
                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        Program.RW_Set_LogLength();
                        break;
                    case ConsoleKey.A:
                        Program.RW_Set_LogThrottle();
                        break;
                    case ConsoleKey.B:
                        Program.RW_Set_Address();
                        break;
                    case ConsoleKey.C:
                        Program.RW_Get_ReadBuffer();
                        break;
                    case ConsoleKey.D:
                        Program.RAM_Get_Data();
                        break;
                    case ConsoleKey.E:
                        Program.RAM_Get_Stream();
                        break;
                    case ConsoleKey.F:
                        API.Clear();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        break;
                }
            }

            API.Free();
        }

        // helper
        private static void LoadFPGAImage()
        {
            Console.Clear();
            Console.WriteLine("Please enter the file path of the FPGA image:");
            Console.WriteLine();

            var filePath = Console.ReadLine();

            while (true)
            {
                if (File.Exists(filePath))
                {
                    API.LoadFPGAImage(filePath);
                    break;
                }
            }
        }

        private static void WriteColored(string text)
        {
            ConsoleColor color;

            color = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ForegroundColor = color;
        }

        private static void PrintDialogFloat(ref double value, double min, double max, string name, string unit = "")
        {
            Console.Clear();
            Console.WriteLine($"max: {min:F2} <= value <= {max:F2} {unit}\ncurrent: {value:F2} {unit}\n");
            Console.WriteLine($"Please enter the desired {name}:");
            Console.WriteLine();

            while (!double.TryParse(Console.ReadLine(), out value) || value < min || value > max)
            {
                //
            }
        }

        private static void PrintDialogInteger(ref uint value, uint min, uint max, string name, string unit = "")
        {
            Console.Clear();
            Console.WriteLine($"max: {min} <= value <= {max} {unit}\ncurrent: {value} {unit}\n");
            Console.WriteLine($"Please enter the desired {name}:");
            Console.WriteLine();

            while (!uint.TryParse(Console.ReadLine(), out value) || value < min || value > max)
            {
                //
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
                    position = API.General.Position;

                    Console.WriteLine($"{position,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void SG_Set_Phase()
        {
            double min = 0;
            double max = API.CLOCK_RATE;
            double value = API.SignalGenerator.Phase / Math.Pow(2, 28) * API.CLOCK_RATE;

            Program.PrintDialogFloat(ref value, min, max, "signal generator frequency", "Hz");

            API.SignalGenerator.Phase = (uint)(value * Math.Pow(2, 28) / API.CLOCK_RATE);
        }

        private static void DA_Toggle_Switch()
        {
            API.DataAcquisition.SwitchEnabled = !API.DataAcquisition.SwitchEnabled;
        }

        private static void DA_Get_Raw()
        {
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
                    (a, b) = API.DataAcquisition.Raw;

                    Console.WriteLine($"a: {a,10}, b: {b,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void PT_Set_LogCountExtremum()
        {
            uint min = 0;
            uint max = (uint)Math.Pow(2, 5) - 1;
            uint value = API.PositionTracker.LogCountExtremum;

            Program.PrintDialogInteger(ref value, min, max, "log count extremum");

            API.PositionTracker.LogCountExtremum = value;
        }

        private static void PT_Set_ShiftExtremum()
        {
            uint min = 0;
            uint max = (uint)Math.Pow(2, 3) - 1;
            uint value = API.PositionTracker.ShiftExtremum;

            Program.PrintDialogInteger(ref value, min, max, "shift extremum");

            API.PositionTracker.ShiftExtremum = value;
        }

        private static void PT_Get_Threshold()
        {
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
                    (a, b) = API.PositionTracker.Threshold;

                    Console.WriteLine($"high: {b,10}, low: {a,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void RW_Toggle_Enable()
        {
            API.RamWriter.Enabled = !API.RamWriter.Enabled;
        }

        private static void RW_Toggle_RequestEnable()
        {
            API.RamWriter.RequestEnabled = !API.RamWriter.RequestEnabled;
        }

        private static void RW_Set_LogLength()
        {
            uint min = 0;
            uint max = (uint)Math.Pow(2, 5) - 1;
            uint value = API.RamWriter.LogLength;

            Program.PrintDialogInteger(ref value, min, max, "log length");

            API.RamWriter.LogLength = value;
        }

        private static void RW_Set_LogThrottle()
        {
            uint min = 1;
            uint max = (uint)Math.Pow(2, 5) - 1;
            uint value = API.RamWriter.LogThrottle;

            Program.PrintDialogInteger(ref value, min, max, "log throttle");

            API.RamWriter.LogThrottle = value;
        }

        private static void RW_Set_Address()
        {
            API.RamWriter.Address = API.DATA_BASE;
        }

        private static void RW_Get_ReadBuffer()
        {
            uint address;

            address = API.RamWriter.ReadBuffer;

            Console.Clear();
            Console.WriteLine($"0x{address,8:X}");
            Console.ReadKey(true);
        }  

        private static void RAM_Get_Data()
        {
            Span<int> dataSet;

            dataSet = API.GetBuffer((int)(API.RamWriter.ReadBuffer - API.DATA_BASE));

            foreach (int data in dataSet)
            {
                Console.WriteLine($"{data,10}");
            }

            Console.ReadKey(true);
        }

        private static void RAM_Get_Stream()
        {
            Task task;
            CancellationTokenSource cts;

            cts = new CancellationTokenSource();

            Console.Clear();

            task = Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    int data;

                    API.RamWriter.RequestEnabled = true;
                    data = API.Ram.GetData((int)(API.RamWriter.ReadBuffer - API.DATA_BASE));
                    var buffer = API.RamWriter.ReadBuffer;
                    API.RamWriter.RequestEnabled = false;

                    Console.WriteLine($"{API.RamWriter.ReadBuffer,8:X} - {data,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }
    }
}
