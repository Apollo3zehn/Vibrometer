using Vibrometer.Infrastructure.API;

namespace Vibrometer.WebClient.Model
{
    public class ChartSettings
    {
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

        #region Properties

        // These are properties to allow proper (de)serialization in the browser.
        public double YMin { get; set; }
        public double YMax { get; set; }
        public int Source { get; set; }
        public int LimitMode { get; set; }

        #endregion
    }
}
