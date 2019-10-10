using System.Collections.Generic;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.WebClient.Model
{
    public class AppState
    {
        #region Constructors

        public AppState()
        {
            this.FpgaSettings = new FpgaSettings();

            this.ChartSettingsSet = new List<ChartSettings>()
            {
                new ChartSettings(ApiSource.NoSource, ChartLimitMode.Auto),
                new ChartSettings(ApiSource.Raw, ChartLimitMode.Custom),
                new ChartSettings(ApiSource.Position, ChartLimitMode.Auto),
                new ChartSettings(ApiSource.Filter, ChartLimitMode.Auto),
                new ChartSettings(ApiSource.FourierTransform, ChartLimitMode.ZeroToMax)
            };
        }

        #endregion

        #region Properties

        // These are properties to allow proper (de)serialization in the browser.
        public FpgaSettings FpgaSettings { get; set; }
        public List<ChartSettings> ChartSettingsSet { get; set; }

        #endregion
    }
}
