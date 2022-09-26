using System;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.WebClient.Model
{
    public class SettingsSummary
    {
        #region Constructors

        public SettingsSummary(FpgaSettings fpgaSettings)
        {
            double referenceFrequency;
            int phaseCarrierWidth;
            int factor;

            this.BaseSamplingFrequency = SystemParameters.CLOCK_RATE;

            if (fpgaSettings.SG_FmEnabled)
            {
                phaseCarrierWidth = ApiInfo.Instance[ApiParameter.SG_PhaseCarrier].Size;
                // Phase width of the signal's DDS compiler is 16.
                this.SG_CarrierFrequency = SystemParameters.CLOCK_RATE / Math.Pow(2, phaseCarrierWidth - 16) / Math.Pow(2, fpgaSettings.SG_ShiftCarrier);
            }
            else
            {
                this.SG_CarrierFrequency = fpgaSettings.SG_PhaseCarrier / Math.Pow(2, ApiInfo.Instance[ApiParameter.SG_PhaseCarrier].Size) * SystemParameters.CLOCK_RATE;
            }

            this.SG_SignalFrequency = fpgaSettings.SG_PhaseSignal / Math.Pow(2, ApiInfo.Instance[ApiParameter.SG_PhaseSignal].Size) * SystemParameters.CLOCK_RATE;
            this.PT_ExtremumFrequency = this.BaseSamplingFrequency / Math.Pow(2, fpgaSettings.PT_LogCountExtremum);
            this.FI_SamplingFrequency = this.BaseSamplingFrequency / Math.Pow(2, fpgaSettings.FI_LogThrottle);
            this.FT_SamplingFrequency = this.FI_SamplingFrequency / Math.Pow(2, fpgaSettings.FT_LogThrottle);
            this.FT_FrequencyResolution = this.FT_SamplingFrequency / SystemParameters.FFT_LENGTH;

            switch ((ApiSource)fpgaSettings.AS_Source)
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

            this.RW_SamplingFrequency = referenceFrequency / Math.Pow(2, fpgaSettings.RW_LogThrottle);
            this.RW_BufferFrequency = this.RW_SamplingFrequency / (SystemParameters.FFT_LENGTH * Math.Pow(2, fpgaSettings.FT_LogCountAverages));

            //
            this.Step = 0;
            this.XMin = 0;
            this.XMax = 0;
            this.Unit = string.Empty;

            switch ((ApiSource)fpgaSettings.AS_Source)
            {
                case ApiSource.NoSource:
                    break;
                case ApiSource.Raw:
                case ApiSource.Position:
                case ApiSource.Filter:
                    this.Step = 1 / this.RW_SamplingFrequency;
                    this.XMin = 0 * this.Step;
                    this.XMax = (Math.Pow(2, fpgaSettings.RW_LogLength) - 1) * this.Step;
                    break;
                case ApiSource.FourierTransform:
                    // df = N / f
                    this.Step = this.FT_SamplingFrequency / SystemParameters.FFT_LENGTH;
                    // XMin = -N/2 * df
                    this.XMin = -SystemParameters.FFT_LENGTH / 2 * this.Step;
                    // XMax = +(N/2 - 1) * df
                    this.XMax = (SystemParameters.FFT_LENGTH / 2 - 1) * this.Step;
                    break;
                default:
                    throw new ArgumentException();
            }

            (factor, this.Unit) = this.ConvertUnit(Math.Max(Math.Abs(this.XMin), Math.Abs(this.XMax)), (ApiSource)fpgaSettings.AS_Source);
            this.Step *= Math.Pow(1000, factor);
            this.XMin *= Math.Pow(1000, factor);
            this.XMax *= Math.Pow(1000, factor);
        }

        private (int, string) ConvertUnit(double value, ApiSource source)
        {
            int factor;

            string unit;
            string prefix;
            
            factor = 0;
            prefix = string.Empty;

            switch (source)
            {
                case ApiSource.NoSource:
                case ApiSource.Raw:
                case ApiSource.Position:
                case ApiSource.Filter:
                    while (true)
                    {
                        if (value < 1 && value != 0)
                        {
                            value *= 1000;
                            factor += 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case ApiSource.FourierTransform:
                    while (true)
                    {
                        if (value > 1000 && value != 0)
                        {
                            value /= 1000;
                            factor -= 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

            switch (factor)
            {
                case -2: // megahertz
                    prefix = "M";
                    break;
                case -1: // kilohertz
                    prefix = "k";
                    break;
                case 1: // millisecond
                    prefix = "m";
                    break;
                case 2: // microsecond
                    prefix = "µ";
                    break;
                case 0: // second, hertz
                default:
                    break;
            }

            switch (source)
            {
                case ApiSource.NoSource:
                    unit = string.Empty;
                    break;
                case ApiSource.Raw:
                case ApiSource.Position:
                case ApiSource.Filter:
                    unit = prefix + "s";
                    break;
                case ApiSource.FourierTransform:
                    unit = prefix + "Hz";
                    break;
                default:
                    throw new ArgumentException();
            }

            return (factor, unit);
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

        public double Step { get; }
        public double XMin { get; }
        public double XMax { get; }
        public string Unit { get; }

        #endregion
    }
}
