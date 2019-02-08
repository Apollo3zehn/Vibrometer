using System;
using Vibrometer.BaseTypes;
using Vibrometer.BaseTypes.API;

namespace Vibrometer.WebClient.Model
{
    public class SettingsSummary
    {
        #region Constructors

        public SettingsSummary(VibrometerState vibrometerState)
        {
            uint referenceFrequency;

            this.BaseSamplingFrequency = 125000000;
            this.SG_CarrierFrequency = (uint)(vibrometerState.SG_PhaseCarrier / Math.Pow(2, 27) * SystemParameters.CLOCK_RATE);
            this.SG_SignalFrequency = (uint)(vibrometerState.SG_PhaseSignal / Math.Pow(2, 27) * SystemParameters.CLOCK_RATE);
            this.PT_ExtremumFrequency = (uint)(this.BaseSamplingFrequency / Math.Pow(2, vibrometerState.PT_LogCountExtremum));
            this.FI_SamplingFrequency = (uint)(this.BaseSamplingFrequency / Math.Pow(2, vibrometerState.FI_LogThrottle));
            this.FT_SamplingFrequency = (uint)(this.FI_SamplingFrequency / Math.Pow(2, vibrometerState.FT_LogThrottle));
            this.FT_FrequencyResolution = this.FI_SamplingFrequency / SystemParameters.FFT_LENGTH;

            switch ((ApiSource)vibrometerState.AS_Source)
            {
                case ApiSource.NoSource:
                    referenceFrequency = 0;
                    this.RW_SamplingFrequency = 0;
                    break;
                case ApiSource.Raw:
                    referenceFrequency = this.BaseSamplingFrequency;
                    break;
                case ApiSource.Position:
                    referenceFrequency = this.BaseSamplingFrequency;
                    break;
                case ApiSource.Filter:
                    referenceFrequency = this.FI_SamplingFrequency;
                    break;
                case ApiSource.FourierTransform:
                    referenceFrequency = this.FT_SamplingFrequency;
                    break;
                default:
                    throw new ArgumentException();
            }

            this.RW_SamplingFrequency = (uint)(referenceFrequency / Math.Pow(2, vibrometerState.RW_LogThrottle));
            this.RW_BufferFrequency = (uint)(this.RW_SamplingFrequency / (SystemParameters.FFT_LENGTH * Math.Pow(2, vibrometerState.FT_LogCountAverages)));
        }

        #endregion

        #region Properties

        public uint BaseSamplingFrequency { get; }
        public uint SG_CarrierFrequency { get; }
        public uint SG_SignalFrequency { get; }
        public uint PT_ExtremumFrequency { get; }
        public uint FI_SamplingFrequency { get; }
        public uint FT_SamplingFrequency { get; }
        public uint FT_FrequencyResolution { get; }
        public uint RW_SamplingFrequency { get; }
        public uint RW_BufferFrequency { get; }

        #endregion
    }
}
