using Mono.Unix.Native;
using System;
using System.IO;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.API
{
    public class VibrometerApi : IDisposable
    {
        #region Fields

        private static IntPtr _GPIO;
        private static IntPtr _DATA;
        private static IntPtr _SWITCH;

        #endregion

        #region Constructors

        public VibrometerApi()
        {
            #if LIVE

                var fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);

                _GPIO = Syscall.mmap(IntPtr.Zero, SystemParameters.GPIO_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.GPIO_BASE);
                _DATA = Syscall.mmap(IntPtr.Zero, SystemParameters.DATA_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.DATA_BASE);
                _SWITCH = Syscall.mmap(IntPtr.Zero, SystemParameters.SWITCH_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.SWITCH_BASE);

                Syscall.close(fd);

            #endif

            AxisSwitch = new AxisSwitch(_SWITCH);
            SignalGenerator = new SignalGenerator(_GPIO + SystemParameters.GPIO_SIGNAL_GENERATOR);
            DataAcquisition = new DataAcquisition(_GPIO + SystemParameters.GPIO_DATA_ACQUISITION);
            PositionTracker = new PositionTracker(_GPIO + SystemParameters.GPIO_POSITION_TRACKER);
            Filter = new Filter(_GPIO + SystemParameters.GPIO_FILTER);
            FourierTransform = new FourierTransform(_GPIO + SystemParameters.GPIO_FOURIER_TRANSFORM);
            RamWriter = new RamWriter(_GPIO + SystemParameters.GPIO_RAM_WRITER);
        }

        #endregion

        #region Properties

        public AxisSwitch AxisSwitch { get; }
        public SignalGenerator SignalGenerator { get; }
        public DataAcquisition DataAcquisition { get; }
        public PositionTracker PositionTracker { get; }
        public Filter Filter { get; }
        public FourierTransform FourierTransform { get; }
        public RamWriter RamWriter { get; }

        #endregion

        #region "Methods"

        public void SetDefaults()
        {
            // source
            AxisSwitch.Source = ApiSource.FourierTransform;

            // 1000 Hz
            SignalGenerator.PhaseCarrier = (uint)(1000 * ApiInfo.Instance[ApiParameter.SG_PhaseCarrier].Size / SystemParameters.CLOCK_RATE);

            // approx. 1s
            PositionTracker.LogCountExtremum = 27;

            // central value +- max / (2^4)
            PositionTracker.ShiftExtremum = 4;

            // throttle data by factor 2^12 = 4096 to get into the kHz range
            //Filter.LogThrottle = 12;

            // calculate the average of 2^2 = 4 FFTs
            FourierTransform.LogCountAverages = 2;
            
            // TODO: TBD
            FourierTransform.LogThrottle = 14;

            // physical RAM address
            RamWriter.Address = SystemParameters.DATA_BASE;

            // buffer length = 2^8 = 256 => 256 * 4 byte = 1024 byte
#warning Should be SystemParameters.FftLength?
            RamWriter.LogLength = 8;

            // clear ram
            ClearRam();

            // enable RAM writer
            RamWriter.Enabled = true;

            // enable Fourier Transform
            FourierTransform.Enabled = true;
        }

        public void FillBuffer(Span<int> buffer)
        {
            RamWriter.RequestEnabled = true;

            #if LIVE

                var address = (int)RamWriter.ReadBuffer;

                if (address == 0)
                    return;

                var offset = address - SystemParameters.DATA_BASE;

                unsafe
                {
                    var source = new Span<int>(IntPtr.Add(_DATA, offset).ToPointer(), buffer.Length);
                    source.CopyTo(buffer);
                }

            #else

                var random = new Random();

                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = random.Next(int.MinValue, int.MaxValue);
                }

            #endif

            RamWriter.RequestEnabled = false;
        }

        public void ClearRam()
        {
            Span<byte> buffer;

            var byteCount = (int)Math.Pow(2, RamWriter.LogLength) * SystemParameters.BYTE_COUNT * SystemParameters.BUFFER_COUNT;
            var enabled = RamWriter.Enabled;
            RamWriter.Enabled = false;

            #if LIVE

                unsafe
                {
                    buffer = new Span<byte>((byte*)_DATA, byteCount);
                }

            #else

                buffer = new Span<byte>(new byte[byteCount]);

            #endif

            buffer.Clear();
            RamWriter.Enabled = enabled;
        }

        public void LoadBitstream(string filePath)
        {
            using (var sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                InternalLoadBitstream(sourceFileStream);
            }
        }

        public void LoadBitstream(byte[] bitstream)
        {
            using (var sourceFileStream = new MemoryStream(bitstream))
            {
                InternalLoadBitstream(sourceFileStream);
            }
        }

        private void InternalLoadBitstream(Stream sourceFileStream)
        {
            ApiProxy.IsEnabled = false;

            #if LIVE

                using (var targetFileStream = File.Open("/dev/xdevcfg", FileMode.Open, FileAccess.Write))
                {
                    sourceFileStream.CopyTo(targetFileStream);
                }

            #endif

            ApiProxy.IsEnabled = true;
        }

        public FpgaSettings GetState()
        {
            return new FpgaSettings()
            {
                AS_Source = unchecked((int)AxisSwitch.Source),
                SG_FmEnabled = SignalGenerator.FmEnabled,
                SG_ShiftCarrier = unchecked((int)SignalGenerator.ShiftCarrier),
                SG_PhaseSignal = unchecked((int)SignalGenerator.PhaseSignal),
                SG_PhaseCarrier = unchecked((int)SignalGenerator.PhaseCarrier),
                DA_SwitchEnabled = DataAcquisition.SwitchEnabled,
                PT_LogScale = unchecked((int)PositionTracker.LogScale),
                PT_LogCountExtremum = unchecked((int)PositionTracker.LogCountExtremum),
                PT_ShiftExtremum = unchecked((int)PositionTracker.ShiftExtremum),
                FI_Enabled = Filter.Enabled,
                FI_LogThrottle = unchecked((int)Filter.LogThrottle),
                FT_Enabled = FourierTransform.Enabled,
                FT_LogCountAverages = unchecked((int)FourierTransform.LogCountAverages),
                FT_LogThrottle = unchecked((int)FourierTransform.LogThrottle),
                RW_Enabled = RamWriter.Enabled,
                RW_LogLength = unchecked((int)RamWriter.LogLength),
                RW_LogThrottle = unchecked((int)RamWriter.LogThrottle)
            };
        }

        public void SetState(FpgaSettings fpgaSettings)
        {
            InternalSetStateSafe(fpgaSettings.RW_Enabled, fpgaSettings.FT_Enabled, () =>
            {
                AxisSwitch.Source = (ApiSource)fpgaSettings.AS_Source;
                SignalGenerator.FmEnabled = fpgaSettings.SG_FmEnabled;
                SignalGenerator.ShiftCarrier = unchecked((uint)fpgaSettings.SG_ShiftCarrier);
                SignalGenerator.PhaseSignal = unchecked((uint)fpgaSettings.SG_PhaseSignal);
                SignalGenerator.PhaseCarrier = unchecked((uint)fpgaSettings.SG_PhaseCarrier);
                DataAcquisition.SwitchEnabled = fpgaSettings.DA_SwitchEnabled;
                PositionTracker.LogScale = unchecked((uint)fpgaSettings.PT_LogScale);
                PositionTracker.LogCountExtremum = unchecked((uint)fpgaSettings.PT_LogCountExtremum);
                PositionTracker.ShiftExtremum = unchecked((uint)fpgaSettings.PT_ShiftExtremum);
                Filter.Enabled = fpgaSettings.FI_Enabled;
                Filter.LogThrottle = unchecked((uint)fpgaSettings.FI_LogThrottle);
                FourierTransform.LogCountAverages = unchecked((uint)fpgaSettings.FT_LogCountAverages);
                FourierTransform.LogThrottle = unchecked((uint)fpgaSettings.FT_LogThrottle);
                RamWriter.LogLength = unchecked((uint)fpgaSettings.RW_LogLength);
                RamWriter.LogThrottle = unchecked((uint)fpgaSettings.RW_LogThrottle);
            });
        }

        public void SetStateSafe(Action action)
        {
            InternalSetStateSafe(RamWriter.Enabled, FourierTransform.Enabled, action);
        }

        private void InternalSetStateSafe(bool ramWriter_enabled, bool fourierTransform_enabled, Action action)
        {
            FourierTransform.Enabled = false;
            RamWriter.Enabled = false;

            action?.Invoke();

            ClearRam();

            if (AxisSwitch.Source == ApiSource.FourierTransform)
            {
                RamWriter.LogThrottle = 0;
            }
            
            FourierTransform.Enabled = fourierTransform_enabled;
            RamWriter.RequestEnabled = false;
            RamWriter.Enabled = ramWriter_enabled;
        }

        #endregion

        #region IDisposable

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (_GPIO.ToInt64() > 0)
                {
                    Syscall.munmap(_GPIO, SystemParameters.GPIO_SIZE);
                }

                if (_DATA.ToInt64() > 0)
                {
                    Syscall.munmap(_DATA, SystemParameters.DATA_SIZE);
                }

                if (_SWITCH.ToInt64() > 0)
                {
                    Syscall.munmap(_SWITCH, SystemParameters.SWITCH_SIZE);
                }

                disposedValue = true;
            }
        }

        ~VibrometerApi()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
