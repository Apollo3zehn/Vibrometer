using Mono.Unix.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
            int fd;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);

                _GPIO = Syscall.mmap(IntPtr.Zero, SystemParameters.GPIO_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.GPIO_BASE);
                _DATA = Syscall.mmap(IntPtr.Zero, SystemParameters.DATA_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.DATA_BASE);
                _SWITCH = Syscall.mmap(IntPtr.Zero, SystemParameters.SWITCH_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.SWITCH_BASE);

                Syscall.close(fd);
            }

            this.AxisSwitch = new AxisSwitch(_SWITCH);
            this.SignalGenerator = new SignalGenerator(_GPIO + SystemParameters.GPIO_SIGNAL_GENERATOR);
            this.DataAcquisition = new DataAcquisition(_GPIO + SystemParameters.GPIO_DATA_ACQUISITION);
            this.PositionTracker = new PositionTracker(_GPIO + SystemParameters.GPIO_POSITION_TRACKER);
            this.Filter = new Filter(_GPIO + SystemParameters.GPIO_FILTER);
            this.FourierTransform = new FourierTransform(_GPIO + SystemParameters.GPIO_FOURIER_TRANSFORM);
            this.RamWriter = new RamWriter(_GPIO + SystemParameters.GPIO_RAM_WRITER);
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
            this.AxisSwitch.Source = ApiSource.FourierTransform;

            // 1000 Hz
            this.SignalGenerator.PhaseCarrier = (uint)(1000 * Math.Pow(2, 27) / SystemParameters.CLOCK_RATE);

            // approx. 1s
            this.PositionTracker.LogCountExtremum = 27;

            // central value +- max / (2^4)
            this.PositionTracker.ShiftExtremum = 4;

            // throttle data by factor 2^12 = 4096 to get into the kHz range
            //this.Filter.LogThrottle = 12;

            // calculate the average of 2^2 = 4 FFTs
            this.FourierTransform.LogCountAverages = 2;
            
            // TBD
            this.FourierTransform.LogThrottle = 14;

            // physical RAM address
            this.RamWriter.Address = SystemParameters.DATA_BASE;

            // buffer length = 2^8 = 256 => 256 * 4 byte = 1024 byte
            this.RamWriter.LogLength = 8;

            // clear ram
            this.ClearRam();

            // enable RAM writer
            this.RamWriter.Enabled = true;

            // enable Fourier Transform
            this.FourierTransform.Enabled = true;
        }

        public int[] GetBuffer()
        {
            int length;
            int address;
            int offset;
            int[] buffer;
            Random random;

            this.RamWriter.RequestEnabled = true;

            length = (int)Math.Pow(2, this.RamWriter.LogLength);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                address = (int)this.RamWriter.ReadBuffer;

                if (address == 0)
                {
                    return new int[length];
                }

                offset = address - SystemParameters.DATA_BASE;

                unsafe
                {
                    buffer = new Span<int>(IntPtr.Add(_DATA, offset).ToPointer(), length).ToArray();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                buffer = new int[length];
                random = new Random();

                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = random.Next(int.MinValue, int.MaxValue);
                }
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            this.RamWriter.RequestEnabled = false;

            return buffer;
        }

        public void ClearRam()
        {
            int byteCount;
            bool enabled;
            Span<byte> buffer;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                byteCount = (int)Math.Pow(2, this.RamWriter.LogLength) * SystemParameters.BYTE_COUNT * SystemParameters.BUFFER_COUNT;
                enabled = this.RamWriter.Enabled;
                this.RamWriter.Enabled = false;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    unsafe
                    {
                        buffer = new Span<byte>((byte*)_DATA, byteCount);
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    buffer = new Span<byte>(new byte[byteCount]);
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }

                buffer.Clear();
                this.RamWriter.Enabled = enabled;
            }
        }

        public void LoadBitstream(string filePath)
        {
            using (var sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                this.InternalLoadBitstream(sourceFileStream);
            }
        }

        public void LoadBitstream(byte[] bitstream)
        {
            using (var sourceFileStream = new MemoryStream(bitstream))
            {
                this.InternalLoadBitstream(sourceFileStream);
            }
        }

        private void InternalLoadBitstream(Stream sourceFileStream)
        {
            ApiProxy.IsEnabled = false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var targetFileStream = File.Open("/dev/xdevcfg", FileMode.Open, FileAccess.Write))
                {
                    sourceFileStream.CopyTo(targetFileStream);
                }
            }

            ApiProxy.IsEnabled = true;
        }

        public FpgaSettings GetState()
        {
            return new FpgaSettings()
            {
                AS_Source = unchecked((int)this.AxisSwitch.Source),
                SG_FmEnabled = this.SignalGenerator.FmEnabled,
                SG_PhaseSignal = unchecked((int)this.SignalGenerator.PhaseSignal),
                SG_PhaseCarrier = unchecked((int)this.SignalGenerator.PhaseCarrier),
                DA_SwitchEnabled = this.DataAcquisition.SwitchEnabled,
                PT_LogScale = unchecked((int)this.PositionTracker.LogScale),
                PT_LogCountExtremum = unchecked((int)this.PositionTracker.LogCountExtremum),
                PT_ShiftExtremum = unchecked((int)this.PositionTracker.ShiftExtremum),
                FI_Enabled = this.Filter.Enabled,
                FI_LogThrottle = unchecked((int)this.Filter.LogThrottle),
                FT_Enabled = this.FourierTransform.Enabled,
                FT_LogCountAverages = unchecked((int)this.FourierTransform.LogCountAverages),
                FT_LogThrottle = unchecked((int)this.FourierTransform.LogThrottle),
                RW_Enabled = this.RamWriter.Enabled,
                RW_LogLength = unchecked((int)this.RamWriter.LogLength),
                RW_LogThrottle = unchecked((int)this.RamWriter.LogThrottle)
            };
        }

        public void SetState(FpgaSettings fpgaSettings)
        {
            this.InternalSetStateSafe(fpgaSettings.RW_Enabled, fpgaSettings.FT_Enabled, () =>
            {
                this.AxisSwitch.Source = (ApiSource)fpgaSettings.AS_Source;
                this.SignalGenerator.FmEnabled = fpgaSettings.SG_FmEnabled;
                this.SignalGenerator.PhaseSignal = unchecked((uint)fpgaSettings.SG_PhaseSignal);
                this.SignalGenerator.PhaseCarrier = unchecked((uint)fpgaSettings.SG_PhaseCarrier);
                this.DataAcquisition.SwitchEnabled = fpgaSettings.DA_SwitchEnabled;
                this.PositionTracker.LogScale = unchecked((uint)fpgaSettings.PT_LogScale);
                this.PositionTracker.LogCountExtremum = unchecked((uint)fpgaSettings.PT_LogCountExtremum);
                this.PositionTracker.ShiftExtremum = unchecked((uint)fpgaSettings.PT_ShiftExtremum);
                this.Filter.Enabled = fpgaSettings.FI_Enabled;
                this.Filter.LogThrottle = unchecked((uint)fpgaSettings.FI_LogThrottle);
                this.FourierTransform.LogCountAverages = unchecked((uint)fpgaSettings.FT_LogCountAverages);
                this.FourierTransform.LogThrottle = unchecked((uint)fpgaSettings.FT_LogThrottle);
                this.RamWriter.LogLength = unchecked((uint)fpgaSettings.RW_LogLength);
                this.RamWriter.LogThrottle = unchecked((uint)fpgaSettings.RW_LogThrottle);
            });
        }

        public void SetStateSafe(Action action)
        {
            this.InternalSetStateSafe(this.RamWriter.Enabled, this.FourierTransform.Enabled, action);
        }

        private void InternalSetStateSafe(bool ramWriter_enabled, bool fourierTransform_enabled, Action action)
        {
            this.FourierTransform.Enabled = false;
            this.RamWriter.Enabled = false;

            action?.Invoke();

            this.ClearRam();
            this.FourierTransform.Enabled = fourierTransform_enabled;
            this.RamWriter.RequestEnabled = false;
            this.RamWriter.Enabled = ramWriter_enabled;
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
