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
            double referenceFrequency;

            this.BaseSamplingFrequency = 125000000;
            this.SG_CarrierFrequency = vibrometerState.SG_PhaseCarrier / Math.Pow(2, 27) * SystemParameters.CLOCK_RATE;
            this.SG_SignalFrequency = vibrometerState.SG_PhaseSignal / Math.Pow(2, 27) * SystemParameters.CLOCK_RATE;
            this.PT_ExtremumFrequency = this.BaseSamplingFrequency / Math.Pow(2, vibrometerState.PT_LogCountExtremum);
            this.FI_SamplingFrequency = this.BaseSamplingFrequency / Math.Pow(2, vibrometerState.FI_LogThrottle);
            this.FT_SamplingFrequency = this.FI_SamplingFrequency / Math.Pow(2, vibrometerState.FT_LogThrottle);
            this.FT_FrequencyResolution = this.FT_SamplingFrequency / SystemParameters.FFT_LENGTH;

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

            this.RW_SamplingFrequency = referenceFrequency / Math.Pow(2, vibrometerState.RW_LogThrottle);
            this.RW_BufferFrequency = this.RW_SamplingFrequency / (SystemParameters.FFT_LENGTH * Math.Pow(2, vibrometerState.FT_LogCountAverages));
        }

        #endregion

        #region Properties

        public double BaseSamplingFrequency { get; }
        public double SG_CarrierFrequency { get; }
        public double SG_SignalFrequency { get; }
        public double PT_ExtremumFrequency { get; }
        public double FI_SamplingFrequency { get; }
        public double FT_SamplingFrequency { get; }
        public double FT_FrequencyResolution { get; }
        public double RW_SamplingFrequency { get; }
        public double RW_BufferFrequency { get; }

        #endregion
    }
}
