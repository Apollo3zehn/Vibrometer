using Mono.Unix.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Vibrometer.Shared.API
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                int fd;

                fd = Syscall.open("/dev/mem", OpenFlags.O_RDWR);

                _GPIO = Syscall.mmap(IntPtr.Zero, SystemParameters.GPIO_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.GPIO_BASE);
                _DATA = Syscall.mmap(IntPtr.Zero, SystemParameters.DATA_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.DATA_BASE);
                _SWITCH = Syscall.mmap(IntPtr.Zero, SystemParameters.SWITCH_SIZE, MmapProts.PROT_READ | MmapProts.PROT_WRITE, MmapFlags.MAP_SHARED, fd, SystemParameters.SWITCH_BASE);

                Syscall.close(fd);

                this.General = new Linux.General(_SWITCH);
                this.SignalGenerator = new Linux.SignalGenerator(_GPIO + SystemParameters.GPIO_SIGNAL_GENERATOR);
                this.DataAcquisition = new Linux.DataAcquisition(_GPIO + SystemParameters.GPIO_DATA_ACQUISITION);
                this.PositionTracker = new Linux.PositionTracker(_GPIO + SystemParameters.GPIO_POSITION_TRACKER);
                this.Filter = new Linux.Filter(_GPIO + SystemParameters.GPIO_FILTER);
                this.FourierTransform = new Linux.FourierTransform(_GPIO + SystemParameters.GPIO_FOURIER_TRANSFORM);
                this.RamWriter = new Linux.RamWriter(_GPIO + SystemParameters.GPIO_RAM_WRITER);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.General = new Linux.General(_SWITCH);
                this.SignalGenerator = new Linux.SignalGenerator(_GPIO + SystemParameters.GPIO_SIGNAL_GENERATOR);
                this.DataAcquisition = new Linux.DataAcquisition(_GPIO + SystemParameters.GPIO_DATA_ACQUISITION);
                this.PositionTracker = new Linux.PositionTracker(_GPIO + SystemParameters.GPIO_POSITION_TRACKER);
                this.Filter = new Linux.Filter(_GPIO + SystemParameters.GPIO_FILTER);
                this.FourierTransform = new Linux.FourierTransform(_GPIO + SystemParameters.GPIO_FOURIER_TRANSFORM);
                this.RamWriter = new Linux.RamWriter(_GPIO + SystemParameters.GPIO_RAM_WRITER);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        #endregion

        #region Properties

        public IGeneral General { get; }
        public ISignalGenerator SignalGenerator { get; }
        public IDataAcquisition DataAcquisition { get; }
        public IPositionTracker PositionTracker { get; }
        public IFilter Filter { get; }
        public IFourierTransform FourierTransform { get; }
        public IRamWriter RamWriter { get; }

        #endregion

        #region "Methods"

        public void SetDefaults()
        {
            // source
            this.General.Source = Source.Position;

            // 100 Hz
            this.SignalGenerator.Phase = (uint)(100 * Math.Pow(2, 28) / SystemParameters.CLOCK_RATE);

            // approx. 1s
            this.PositionTracker.LogCountExtremum = 27;

            // central value +- max / (2^4)
            this.PositionTracker.ShiftExtremum = 4;

            // physical RAM address
            this.RamWriter.Address = SystemParameters.DATA_BASE;

            // buffer length = 2^10 = 1024 => 1024 * 4 byte = 4096 byte
            this.RamWriter.LogLength = 10;

            // clear ram
            this.ClearRam();

            // throttle data by factor 2^12 = 4096 to get into the kHz range
            this.RamWriter.LogThrottle = 12;

            // enable RAM writer
            this.RamWriter.Enabled = true;
        }

        public int[] GetBuffer()
        {
            int length;
            int offset;
            int[] buffer;

            this.RamWriter.RequestEnabled = true;

            offset = (int)(this.RamWriter.ReadBuffer - SystemParameters.DATA_BASE);
            length = (int)(Math.Pow(2, this.RamWriter.LogLength) * SystemParameters.BYTE_COUNT);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                unsafe
                {
                    buffer = new Span<int>((int*)IntPtr.Add(_DATA, offset), length).ToArray();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                buffer = new int[length / SystemParameters.BYTE_COUNT];
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

        public void LoadFPGAImage(string filePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (var sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var targetFileStream = File.Open("/dev/xdevcfg", FileMode.Open, FileAccess.Write))
                    {
                        sourceFileStream.CopyTo(targetFileStream);
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
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
