using Vibrometer.Infrastructure.API;

namespace Vibrometer.WebClient.Model
{
    public class ChartSettings
    {
        #region Fields

        public double YMin;
        public double YMax;
        public ChartLimitMode LimitMode;

        #endregion

        #region Constructors

        public ChartSettings(ApiSource source, ChartLimitMode limitMode)
        {
            this.Source = source;
            this.LimitMode = limitMode;
        }

        #endregion

        #region Properties

        public ApiSource Source { get; }

        #endregion
    }
}
