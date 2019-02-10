﻿using System;
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

            //
            this.XMin = 0;
            this.XMax = 0;
            this.Unit = string.Empty;

            switch ((ApiSource)vibrometerState.AS_Source)
            {
                case ApiSource.NoSource:
                    break;
                case ApiSource.Raw:
                case ApiSource.Position:
                case ApiSource.Filter:
                    this.XMax = (Math.Pow(2, vibrometerState.RW_LogLength) - 1) / this.RW_SamplingFrequency;
                    (this.XMax, this.Unit) = this.ConvertUnit(this.XMax, (ApiSource)vibrometerState.AS_Source);
                    break;
                case ApiSource.FourierTransform:
                    this.XMax = this.RW_SamplingFrequency;
                    (this.XMax, this.Unit) = this.ConvertUnit(this.XMax, (ApiSource)vibrometerState.AS_Source);
                    break;
                default:
                    throw new ArgumentException();
            }

            this.Step = this.XMax / (Math.Pow(2, vibrometerState.RW_LogLength) - 1);
        }

        private (double, string) ConvertUnit(double value, ApiSource source)
        {
            int factor;

            string unit;
            string prefix_1;
            string prefix_2;
            
            factor = 0;
            prefix_1 = string.Empty;
            prefix_2 = string.Empty;

            switch (source)
            {
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
                            factor += 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case ApiSource.NoSource:
                default:
                    throw new ArgumentException();
            }

            switch (factor)
            {
                case 1: // millisecond, kilohertz
                    prefix_1 = "m";
                    prefix_2 = "k";
                    break;
                case 2: // microsecond, megahertz
                    prefix_1 = "µ";
                    prefix_2 = "M";
                    break;
                case 0: // second, hertz
                default:
                    break;
            }

            switch (source)
            {
                case ApiSource.Raw:
                case ApiSource.Position:
                case ApiSource.Filter:
                    unit = prefix_1 + "s";
                    break;
                case ApiSource.FourierTransform:
                    unit = prefix_2 + "Hz";
                    break;
                case ApiSource.NoSource:
                default:
                    throw new ArgumentException();
            }

            return (value, unit);
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