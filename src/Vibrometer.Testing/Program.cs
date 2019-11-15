using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Vibrometer.API;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

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
                Console.WriteLine($"[Y] - load bitstream");
                Console.WriteLine($"[Z] - set defaults");

                Console.WriteLine();

                Program.WriteColored("AXIS Switch\n", ConsoleColor.Cyan);

                switch (_api.AxisSwitch.Source)
                {
                    case ApiSource.Raw:
                        Program.WriteColored($"[0] - set source (source: raw)\n");
                        break;
                    case ApiSource.Position:
                        Program.WriteColored($"[0] - set source (source: position)\n");
                        break;
                    case ApiSource.Filter:
                        Program.WriteColored($"[0] - set source (source: filter)\n");
                        break;
                    case ApiSource.FourierTransform:
                        Program.WriteColored($"[0] - set source (source: Fourier)\n");
                        break;
                    default:
                        Console.WriteLine($"[0] - set source (source: disabled)");
                        break;
                }

                Console.WriteLine();

                Program.WriteColored("Signal Generator\n", ConsoleColor.Cyan);

                if (_api.SignalGenerator.FmEnabled)
                    Program.WriteColored($"[1] - disable frequency modulation\n");
                else
                    Console.Write($"[1] - enable frequency modulation\n");

                Console.WriteLine($"[2] - set shift carrier");
                Console.WriteLine($"[3] - set phase signal");
                Console.WriteLine($"[4] - set phase carrier");

                Console.WriteLine();

                Program.WriteColored("Data Acquisition\n", ConsoleColor.Cyan);

                if (_api.DataAcquisition.SwitchEnabled)
                    Program.WriteColored($"[5] - disable switch\n");
                else
                    Console.Write($"[5] - enable switch\n");

                Console.WriteLine();

                Program.WriteColored("Position Tracker\n", ConsoleColor.Cyan);
                Console.WriteLine($"[6] - set log scale");
                Console.WriteLine($"[7] - set log count extremum");
                Console.WriteLine($"[8] - set shift extremum");
                Console.WriteLine($"[9] - get threshold");

                Console.WriteLine();

                Program.WriteColored("Filter\n", ConsoleColor.Cyan);
                Console.WriteLine($"[A] - set log throttle");

                Console.WriteLine();

                Program.WriteColored("Fourier Transform\n", ConsoleColor.Cyan);
                if (_api.FourierTransform.Enabled)
                    Program.WriteColored($"[B] - disable Fourier transform\n");
                else
                    Console.Write($"[B] - enable Fourier Transform\n");

                Console.WriteLine($"[C] - set log count averages");
                Console.WriteLine($"[D] - set log throttle");

                Console.WriteLine();

                Program.WriteColored("RAM Writer\n", ConsoleColor.Cyan);

                if (_api.RamWriter.Enabled)
                    Program.WriteColored($"[E] - disable RAM writer\n");
                else
                    Console.Write($"[E] - enable RAM writer\n");

                if (_api.RamWriter.RequestEnabled)
                    Program.WriteColored($"[F] - disable buffer request\n");
                else
                    Console.Write($"[F] - enable buffer request\n");

                Console.WriteLine($"[G] - set log length");
                Console.WriteLine($"[H] - set log throttle");
                Console.WriteLine($"[I] - get read buffer");

                Console.WriteLine();

                Program.WriteColored("RAM\n", ConsoleColor.Cyan);
                Console.WriteLine($"[J] - get data ({Math.Min(Math.Pow(2, _api.RamWriter.LogLength), 1024)} values)");
                Console.WriteLine($"[K] - get stream");
                Console.WriteLine($"[L] - clear");

                var keyInfo = Console.ReadKey(true);

                switch(keyInfo.Key)
                {
                    case ConsoleKey.Y:
                        Program.LoadBitstream();
                        break;
                    case ConsoleKey.Z:
                        _api.SetDefaults();
                        break;
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.D0:
                        Program.GE_Set_Source();
                        break;
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        Program.SG_Toggle_FM();
                        break;
                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        Program.SG_Set_Shift_Carrier();
                        break;
                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        Program.SG_Set_Phase_Signal();
                        break;
                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        Program.SG_Set_Phase_Carrier();
                        break;
                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        Program.DA_Toggle_Switch();
                        break;
                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        Program.PT_Set_LogScale();
                        break;
                    case ConsoleKey.NumPad7:
                    case ConsoleKey.D7:
                        Program.PT_Set_LogCountExtremum();
                        break;
                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        Program.PT_Set_ShiftExtremum();
                        break;
                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        Program.PT_Get_Threshold();
                        break;
                    case ConsoleKey.A:
                        Program.FI_Set_LogThrottle();
                        break;
                    case ConsoleKey.B:
                        Program.FT_Toggle_Enable();
                        break;
                    case ConsoleKey.C:
                        Program.FT_Set_LogCountAverages();
                        break;
                    case ConsoleKey.D:
                        Program.FT_Set_LogThrottle();
                        break;
                    case ConsoleKey.E:
                        Program.RW_Toggle_Enable();
                        break;
                    case ConsoleKey.F:
                        Program.RW_Toggle_RequestEnable();
                        break;
                    case ConsoleKey.G:
                        Program.RW_Set_LogLength();
                        break;
                    case ConsoleKey.H:
                        Program.RW_Set_LogThrottle();
                        break;
                    case ConsoleKey.I:
                        Program.RW_Get_ReadBuffer();
                        break;
                    case ConsoleKey.J:
                        Program.RAM_Get_Data();
                        break;
                    case ConsoleKey.K:
                        Program.RAM_Get_Stream();
                        break;
                    case ConsoleKey.L:
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
        private static void LoadBitstream()
        {
            Console.Clear();
            Console.WriteLine("Please enter the bitstream file path:");
            Console.WriteLine();

            var filePath = Console.ReadLine();

            while (true)
            {
                if (File.Exists(filePath))
                {
                    _api.LoadBitstream(filePath);
                    break;
                }
            }
        }

        private static void WriteColored(string text, ConsoleColor color = ConsoleColor.Green)
        {
            ConsoleColor original;

            original = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = original;
        }

        private static void PrintDialogFloat(ApiParameter parameter, ref double value, double min, double max, string unit)
        {
            ApiRecord record;

            record = ApiInfo.Instance[parameter];

            Console.Clear();
            Console.WriteLine($"{min:F2} <= value <= {max:F2} {unit}\ncurrent: {value:F2} {unit}\n");
            Console.WriteLine($"Please enter the desired '{record.DisplayName}':");
            Console.WriteLine();

            while (!double.TryParse(Console.ReadLine(), out value) || value < min || value > max)
            {
                //
            }
        }

        private static void PrintDialogInteger(ApiParameter parameter, ref uint value)
        {
            ApiRecord record;

            record = ApiInfo.Instance[parameter];

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
            uint value = (uint)_api.AxisSwitch.Source;

            // TODO: max is 4
            Program.PrintDialogInteger(ApiParameter.AS_Source, ref value);

            _api.AxisSwitch.Source = (ApiSource)value;
        }

        private static void SG_Toggle_FM()
        {
            _api.SignalGenerator.FmEnabled = !_api.SignalGenerator.FmEnabled;
        }

        private static void SG_Set_Shift_Carrier()
        {
            uint value = _api.SignalGenerator.ShiftCarrier;

            Program.PrintDialogInteger(ApiParameter.SG_ShiftCarrier, ref value);

            _api.SignalGenerator.ShiftCarrier = value;
        }

        private static void SG_Set_Phase_Signal()
        {
            double min = 0;
            double max = SystemParameters.CLOCK_RATE;
            double value = _api.SignalGenerator.PhaseSignal / Math.Pow(2, ApiInfo.Instance[ApiParameter.SG_PhaseSignal].Size) * SystemParameters.CLOCK_RATE;

            Program.PrintDialogFloat(ApiParameter.SG_PhaseSignal, ref value, min, max, "Hz");

            _api.SignalGenerator.PhaseSignal = (uint)(value * Math.Pow(2, ApiInfo.Instance[ApiParameter.SG_PhaseSignal].Size) / SystemParameters.CLOCK_RATE);
        }

        private static void SG_Set_Phase_Carrier()
        {
            double min = 0;
            double max = SystemParameters.CLOCK_RATE;
            double value = _api.SignalGenerator.PhaseCarrier / Math.Pow(2, ApiInfo.Instance[ApiParameter.SG_PhaseCarrier].Size) * SystemParameters.CLOCK_RATE;

            Program.PrintDialogFloat(ApiParameter.SG_PhaseCarrier, ref value, min, max, "Hz");

            _api.SignalGenerator.PhaseCarrier = (uint)(value * Math.Pow(2, ApiInfo.Instance[ApiParameter.SG_PhaseCarrier].Size) / SystemParameters.CLOCK_RATE);
        }

        private static void DA_Toggle_Switch()
        {
            _api.DataAcquisition.SwitchEnabled = !_api.DataAcquisition.SwitchEnabled;
        }

        private static void PT_Set_LogScale()
        {
            uint value = _api.PositionTracker.LogScale;

            Program.PrintDialogInteger(ApiParameter.PT_LogScale, ref value);

            _api.PositionTracker.LogScale = value;
        }

        private static void PT_Set_LogCountExtremum()
        {
            uint value = _api.PositionTracker.LogCountExtremum;

            Program.PrintDialogInteger(ApiParameter.PT_LogCountExtremum, ref value);

            _api.PositionTracker.LogCountExtremum = value;
        }

        private static void PT_Set_ShiftExtremum()
        {
            uint value = _api.PositionTracker.ShiftExtremum;

            Program.PrintDialogInteger(ApiParameter.PT_ShiftExtremum, ref value);

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

            try
            {
                task.Wait();
            }
            catch (Exception ex) when (ex.InnerException.GetType() == typeof(TaskCanceledException))
            {
                //
            }
        }

        private static void FI_Set_LogThrottle()
        {
            uint value = _api.Filter.LogThrottle;

            Program.PrintDialogInteger(ApiParameter.FI_LogThrottle, ref value);

            _api.Filter.LogThrottle = value;
        }

        private static void FT_Toggle_Enable()
        {
            _api.FourierTransform.Enabled = !_api.FourierTransform.Enabled;
        }

        private static void FT_Set_LogCountAverages()
        {
            uint value = _api.FourierTransform.LogCountAverages;

            Program.PrintDialogInteger(ApiParameter.FT_LogCountAverages, ref value);

            _api.FourierTransform.LogCountAverages = value;
        }

        private static void FT_Set_LogThrottle()
        {
            uint value = _api.FourierTransform.LogThrottle;

            Program.PrintDialogInteger(ApiParameter.FT_LogThrottle, ref value);

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

            Program.PrintDialogInteger(ApiParameter.RW_LogLength, ref value);

            _api.RamWriter.LogLength = value;
        }

        private static void RW_Set_LogThrottle()
        {
            uint value = _api.RamWriter.LogThrottle;

            Program.PrintDialogInteger(ApiParameter.RW_LogThrottle, ref value);

            _api.RamWriter.LogThrottle = value;
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
            ApiSource source;
            uint bufferAddress;
            Span<int> buffer;
            short a;
            short b;

            source = _api.AxisSwitch.Source;
            bufferAddress = _api.RamWriter.ReadBuffer;

            _api.RamWriter.RequestEnabled = true;
            buffer = _api.GetBuffer();
            _api.RamWriter.RequestEnabled = false;

            for (int i = 0; i < Math.Min(buffer.Length, 256); i++)
            {
                a = unchecked((short)(buffer[i] & 0x0000FFFF));
                b = unchecked((short)(buffer[i] >> 16));

                switch (source)
                {
                    case ApiSource.Raw:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Raw data: {b,10} (b), {a,10} (a)");
                        break;
                    case ApiSource.Position:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Position: {buffer[i],10}");
                        break;
                    case ApiSource.Filter:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Filter: {a,10}");
                        break;
                    case ApiSource.FourierTransform:
                        Console.WriteLine($"Buffer: {bufferAddress,8:X} | Raw data: {b,10} (imag), {a,10} (real)");
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
                    short a;
                    short b;
                    Span<int> buffer;

                    _api.RamWriter.RequestEnabled = true;
                    buffer = _api.GetBuffer();
                    _api.RamWriter.RequestEnabled = false;

                    a = unchecked((short)(buffer[0] & 0x0000FFFF));
                    b = unchecked((short)(buffer[0] >> 16));

                    switch (_api.AxisSwitch.Source)
                    {
                        case ApiSource.Raw:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Raw data: {b,10} (b), {a,10} (a)");
                            break;
                        case ApiSource.Position:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Position: {buffer[0],10}");
                            break;
                        case ApiSource.Filter:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Filter: {a,10}");
                            break;
                        case ApiSource.FourierTransform:
                            Console.WriteLine($"Buffer: {_api.RamWriter.ReadBuffer,8:X} | Raw data: {b,10} (imag), {a,10} (real)");
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

            try
            {
                task.Wait();
            }
            catch (Exception ex) when (ex.InnerException.GetType() == typeof(TaskCanceledException))
            {
                //
            }
        }
    }
}
