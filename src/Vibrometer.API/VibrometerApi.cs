﻿using Mono.Unix.Native;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Vibrometer.BaseTypes.API
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
            int offset;
            int[] buffer;
            Random random;

            this.RamWriter.RequestEnabled = true;

            offset = (int)(this.RamWriter.ReadBuffer - SystemParameters.DATA_BASE);
            length = (int)Math.Pow(2, this.RamWriter.LogLength);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
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
                    buffer[i] = random.Next(0, 100);
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

        public void LoadFpgaImage(string filePath)
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
        }

        public VibrometerState GetState()
        {
            return new VibrometerState()
            {
                AS_Source = (int)this.AxisSwitch.Source,
                SG_FmEnabled = this.SignalGenerator.FmEnabled,
                SG_PhaseSignal = (int)this.SignalGenerator.PhaseSignal,
                SG_PhaseCarrier = (int)this.SignalGenerator.PhaseCarrier,
                DA_SwitchEnabled = this.DataAcquisition.SwitchEnabled,
                PT_LogScale = (int)this.PositionTracker.LogScale,
                PT_LogCountExtremum = (int)this.PositionTracker.LogCountExtremum,
                PT_ShiftExtremum = (int)this.PositionTracker.ShiftExtremum,
                FI_LogThrottle = (int)this.Filter.LogThrottle,
                FT_Enabled = this.FourierTransform.Enabled,
                FT_LogCountAverages = (int)this.FourierTransform.LogCountAverages,
                FT_LogThrottle = (int)this.FourierTransform.LogThrottle,
                RW_Enabled = this.RamWriter.Enabled,
                RW_LogLength = (int)this.RamWriter.LogLength,
                RW_LogThrottle = (int)this.RamWriter.LogThrottle
            };
        }

        public void SetState(VibrometerState vibrometerState)
        {
            this.RamWriter.Enabled = false;
            this.FourierTransform.Enabled = false;

            this.AxisSwitch.Source = (ApiSource)vibrometerState.AS_Source;
            this.SignalGenerator.FmEnabled = vibrometerState.SG_FmEnabled;
            this.SignalGenerator.PhaseSignal = (uint)vibrometerState.SG_PhaseSignal;
            this.SignalGenerator.PhaseCarrier = (uint)vibrometerState.SG_PhaseCarrier;
            this.DataAcquisition.SwitchEnabled = vibrometerState.DA_SwitchEnabled;
            this.PositionTracker.LogScale = (uint)vibrometerState.PT_LogScale;
            this.PositionTracker.LogCountExtremum = (uint)vibrometerState.PT_LogCountExtremum;
            this.PositionTracker.ShiftExtremum = (uint)vibrometerState.PT_ShiftExtremum;
            this.Filter.LogThrottle = (uint)vibrometerState.FI_LogThrottle;
            this.FourierTransform.LogCountAverages = (uint)vibrometerState.FT_LogCountAverages;
            this.FourierTransform.LogThrottle = (uint)vibrometerState.FT_LogThrottle;
            this.RamWriter.LogLength = (uint)vibrometerState.RW_LogLength;
            this.RamWriter.LogThrottle = (uint)vibrometerState.RW_LogThrottle;

            this.RamWriter.RequestEnabled = false;
            this.RamWriter.Enabled = vibrometerState.FT_Enabled;
            this.FourierTransform.Enabled = vibrometerState.RW_Enabled;
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
