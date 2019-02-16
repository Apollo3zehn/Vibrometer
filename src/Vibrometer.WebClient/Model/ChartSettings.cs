using Vibrometer.Infrastructure.API;

namespace Vibrometer.WebClient.Model
{
    public class ChartSettings
    {
        #region Fields

        public double YMin;
        public double YMax;
        public int Source;
        public int LimitMode;

        #endregion

        #region Constructors

        public ChartSettings()
        {
            //
        }

        public ChartSettings(ApiSource source, ChartLimitMode limitMode)
        {
            this.YMin = 0;
            this.YMax = 0;

            this.Source = (int)source;
            this.LimitMode = (int)limitMode;
        }

        #endregion
    }
}
