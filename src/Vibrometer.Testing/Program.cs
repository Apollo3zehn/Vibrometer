using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Vibrometer.Shared;
using Vibrometer.Shared.API;

namespace Vibrometer.Testing
{
    class Program
    {
        static VibrometerApi _api;

        static void Main(string[] args)
        {
            bool exit = false;

            _api = new VibrometerApi();
        
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine($"[L] - load FPGA image");
                Console.WriteLine($"[P] - set defaults");

                Console.WriteLine();

                Console.WriteLine("General");

                switch (_api.General.Source)
                {
                    case Source.Raw:
                        Program.WriteColored($"[0] - set source (source: raw)\n");
                        break;
                    case Source.Position:
                        Program.WriteColored($"[0] - set source (source: position)\n");
                        break;
                    case Source.Filter:
                        Program.WriteColored($"[0] - set source (source: filter)\n");
                        break;
                    case Source.FourierTransform:
                        Program.WriteColored($"[0] - set source (source: Fourier)\n");
                        break;
                    default:
                        Console.WriteLine($"[0] - set source (source: disabled)");
                        break;
                }

                Console.WriteLine();

                Console.WriteLine("Signal Generator");
                Console.WriteLine($"[1] - set phase");

                Console.WriteLine();

                Console.WriteLine("Data Acquisition");

                if (_api.DataAcquisition.SwitchEnabled)
                    Program.WriteColored($"[2] - disable switch\n");
                else
                    Console.Write($"[2] - enable switch\n");

                Console.WriteLine();

                Console.WriteLine("Position Tracker");
                Console.WriteLine($"[3] - set log scale");
                Console.WriteLine($"[4] - set log count extremum");
                Console.WriteLine($"[5] - set shift extremum");
                Console.WriteLine($"[6] - get threshold");

                Console.WriteLine();

                Console.WriteLine("Filter");
                Console.WriteLine($"[7] - set log throttle");

                Console.WriteLine();

                Console.WriteLine("Fourier Transform");
                if (_api.FourierTransform.Enabled)
                    Program.WriteColored($"[8] - disable Fourier Transform\n");
                else
                    Console.Write($"[8] - enable Fourier Transform\n");

                Console.WriteLine($"[9] - set log count averages");
                Console.WriteLine($"[A] - set log throttle");

                Console.WriteLine();

                Console.WriteLine("RAM Writer");

                if (_api.RamWriter.Enabled)
                    Program.WriteColored($"[B] - disable RAM writer\n");
                else
                    Console.Write($"[B] - enable RAM writer\n");

                if (_api.RamWriter.RequestEnabled)
                    Program.WriteColored($"[C] - disable buffer request\n");
                else
                    Console.Write($"[C] - enable buffer request\n");

                Console.WriteLine($"[D] - set log length");
                Console.WriteLine($"[E] - set log throttle");
                Console.WriteLine($"[F] - set address");
                Console.WriteLine($"[G] - get read buffer");

                Console.WriteLine();

                Console.WriteLine("RAM");
                Console.WriteLine($"[H] - get data ({Math.Min(Math.Pow(2, _api.RamWriter.LogLength), 1024)} values)");
                Console.WriteLine($"[I] - get stream");
                Console.WriteLine($"[J] - clear");

                var keyInfo = Console.ReadKey(true);

                switch(keyInfo.Key)
                {
                    case ConsoleKey.L:
                        Program.LoadFPGAImage();
                        break;
                    case ConsoleKey.P:
                        _api.SetDefaults();
                        break;
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.D0:
                        Program.GE_Set_Source();
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
                        Program.PT_Set_LogScale();
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
                        Program.FI_Set_LogThrottle();
                        break;
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        Program.FT_Toggle_Enable();
                        break;
                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        Program.FT_Set_LogCountAverages();
                        break;
                    case ConsoleKey.A:
                        Program.FT_Set_LogThrottle();
                        break;
                    case ConsoleKey.B:
                        Program.RW_Toggle_Enable();
                        break;
                    case ConsoleKey.C:
                        Program.RW_Toggle_RequestEnable();
                        break;
                    case ConsoleKey.D:
                        Program.RW_Set_LogLength();
                        break;
                    case ConsoleKey.E:
                        Program.RW_Set_LogThrottle();
                        break;
                    case ConsoleKey.F:
                        Program.RW_Set_Address();
                        break;
                    case ConsoleKey.G:
                        Program.RW_Get_ReadBuffer();
                        break;
                    case ConsoleKey.H:
                        Program.RAM_Get_Data();
                        break;
                    case ConsoleKey.I:
                        Program.RAM_Get_Stream();
                        break;
                    case ConsoleKey.J:
                        _api.ClearRam();
                        break;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                    default:
                        break;
                }
            }

            _api.Dispose();
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
                    _api.LoadFPGAImage(filePath);
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

        private static void PrintDialogFloat(ApiMethod method, ref double value, double min, double max, string unit)
        {
            ApiRecord record;

            record = SystemParameters.ApiInfo[method];

            Console.Clear();
            Console.WriteLine($"{min:F2} <= value <= {max:F2} {unit}\ncurrent: {value:F2} {unit}\n");
            Console.WriteLine($"Please enter the desired '{record.DisplayName}':");
            Console.WriteLine();

            while (!double.TryParse(Console.ReadLine(), out value) || value < min || value > max)
            {
                //
            }
        }

        private static void PrintDialogInteger(ApiMethod method, ref uint value)
        {
            ApiRecord record;

            record = SystemParameters.ApiInfo[method];

            Console.Clear();
            Console.WriteLine($"{record.Min} <= value <= {record.Max}\ncurrent: {value}\n");
            Console.WriteLine($"Please enter the desired '{record.DisplayName}':");
            Console.WriteLine();

            while (!uint.TryParse(Console.ReadLine(), out value) || value < record.Min || value > record.Max)
            {
                //
            }
        }

        // API
        private static void GE_Set_Source()
        {
            uint value = (uint)_api.General.Source;

            // TODO: max is 4
            Program.PrintDialogInteger(ApiMethod.GE_Source, ref value);

            _api.General.Source = (Source)value;
        }

        private static void SG_Set_Phase()
        {
            double min = 0;
            double max = SystemParameters.CLOCK_RATE;
            double value = _api.SignalGenerator.Phase / Math.Pow(2, 28) * SystemParameters.CLOCK_RATE;

            Program.PrintDialogFloat(ApiMethod.SG_Phase, ref value, min, max, "Hz");

            _api.SignalGenerator.Phase = (uint)(value * Math.Pow(2, 28) / SystemParameters.CLOCK_RATE);
        }

        private static void DA_Toggle_Switch()
        {
            _api.DataAcquisition.SwitchEnabled = !_api.DataAcquisition.SwitchEnabled;
        }

        private static void PT_Set_LogScale()
        {
            uint value = _api.PositionTracker.LogScale;

            Program.PrintDialogInteger(ApiMethod.PT_LogScale, ref value);

            _api.PositionTracker.LogScale = value;
        }

        private static void PT_Set_LogCountExtremum()
        {
            uint value = _api.PositionTracker.LogCountExtremum;

            Program.PrintDialogInteger(ApiMethod.PT_LogCountExtremum, ref value);

            _api.PositionTracker.LogCountExtremum = value;
        }

        private static void PT_Set_ShiftExtremum()
        {
            uint value = _api.PositionTracker.ShiftExtremum;

            Program.PrintDialogInteger(ApiMethod.PT_ShiftExtremum, ref value);

            _api.PositionTracker.ShiftExtremum = value;
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
                    (a, b) = _api.PositionTracker.Threshold;

                    Console.WriteLine($"high: {b,10}, low: {a,10}");
                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }

        private static void FI_Set_LogThrottle()
        {
            uint value = _api.Filter.LogThrottle;

            Program.PrintDialogInteger(ApiMethod.FI_LogThrottle, ref value);

            _api.Filter.LogThrottle = value;
        }

        private static void FT_Toggle_Enable()
        {
            _api.FourierTransform.Enabled = !_api.FourierTransform.Enabled;
        }

        private static void FT_Set_LogCountAverages()
        {
            uint value = _api.FourierTransform.LogCountAverages;

            Program.PrintDialogInteger(ApiMethod.FT_LogCountAverages, ref value);

            _api.FourierTransform.LogCountAverages = value;
        }

        private static void FT_Set_LogThrottle()
        {
            uint value = _api.FourierTransform.LogThrottle;

            Program.PrintDialogInteger(ApiMethod.FT_LogThrottle, ref value);

            _api.FourierTransform.LogThrottle = value;
        }

        private static void RW_Toggle_Enable()
        {
            _api.RamWriter.Enabled = !_api.RamWriter.Enabled;
        }

        private static void RW_Toggle_RequestEnable()
        {
            _api.RamWriter.RequestEnabled = !_api.RamWriter.RequestEnabled;
        }

        private static void RW_Set_LogLength()
        {
            uint value = _api.RamWriter.LogLength;

            Program.PrintDialogInteger(ApiMethod.RW_LogLength, ref value);

            _api.RamWriter.LogLength = value;
        }

        private static void RW_Set_LogThrottle()
        {
            uint value = _api.RamWriter.LogThrottle;

            Program.PrintDialogInteger(ApiMethod.RW_LogThrottle, ref value);

            _api.RamWriter.LogThrottle = value;
        }

        private static void RW_Set_Address()
        {
            _api.RamWriter.Address = SystemParameters.DATA_BASE;
        }

        private static void RW_Get_ReadBuffer()
        {
            uint address;

            address = _api.RamWriter.ReadBuffer;

            Console.Clear();
            Console.WriteLine($"0x{address,8:X}");
            Console.ReadKey(true);
        }  

        private static void RAM_Get_Data()
        {
            Source source;
            uint bufferAddress;
            Span<int> buffer;
            ushort a;
            ushort b;

            source = _api.General.Source;
            bufferAddress = _api.RamWriter.ReadBuffer;

            _api.RamWriter.RequestEnabled = true;
            buffer = _api.GetBuffer();
            _api.RamWriter.RequestEnabled = false;

            for (int i = 0; i < Math.Min(buffer.Length, 128); i++)
            {
                switch (source)
                {
                    case Source.Raw:
                        a = unchecked((ushort)(buffer[i] & ~0xFFFF0000));
                        b = unchecked((ushort)(buffer[i] >> 16));
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Raw data: {(short)b,10} (b), {(short)a,10} (a)");
                        break;
                    case Source.Position:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Position: {buffer[i],10}");
                        break;
                    case Source.Filter:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Filter: {buffer[i],10}");
                        break;
                    case Source.FourierTransform:
                        a = unchecked((ushort)(buffer[0] & ~0xFFFF0000));
                        b = unchecked((ushort)(buffer[0] >> 16));
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Raw data: {(short)b,10} (imag), {(short)a,10} (real)");
                        break;
                    default:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | (undefined): {buffer[i],10}");
                        break;
                }
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
                    ushort a;
                    ushort b;
                    Span<int> buffer;

                    _api.RamWriter.RequestEnabled = true;
                    buffer = _api.GetBuffer();
                    _api.RamWriter.RequestEnabled = false;

                    switch (_api.General.Source)
                    {
                        case Source.Raw:
                            a = unchecked((ushort)(buffer[0] & ~0xFFFF0000));
                            b = unchecked((ushort)(buffer[0] >> 16));
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Raw data: {(short)b,10} (b), {(short)a,10} (a)");
                            break;
                        case Source.Position:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Position: {buffer[0],10}");
                            break;
                        case Source.Filter:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Filter: {buffer[0],10}");
                            break;
                        case Source.FourierTransform:
                            a = unchecked((ushort)(buffer[0] & ~0xFFFF0000));
                            b = unchecked((ushort)(buffer[0] >> 16));
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Raw data: {(short)b,10} (imag), {(short)a,10} (real)");
                            break;
                        default:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | (undefined): {buffer[0],10}");
                            break;
                    }

                    Thread.Sleep(100);
                }
            }, cts.Token);

            Console.ReadKey(true);
            cts.Cancel();
            task.Wait();
        }
    }
}
