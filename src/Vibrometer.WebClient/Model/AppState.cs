using System.Collections.Generic;
using Vibrometer.BaseTypes;

namespace Vibrometer.WebClient.Model
{
    public class AppState : BindableBase
    {
        #region Field

        VibrometerState _vibrometerState;

        #endregion

        #region Constructors

        public AppState()
        {
            this.VibrometerState = new VibrometerState();

            this.PageDescriptionSet = new List<PageDescription>()
            {
                new PageDescription("Page Test 1", "P1", "dashboard", "pagetest1", new List<PageDescription>() { new PageDescription("Page Test 1 Sub Page", "S1", "dashboard", "pagetest1/subpage") }),
                new PageDescription("Settings", "SE", "dashboard", "settings")
            };
        }

        #endregion

        #region Properties

        public VibrometerState VibrometerState
        {
            get { return _vibrometerState; }
            set { base.SetProperty(ref _vibrometerState, value); }
        }

        public List<PageDescription> PageDescriptionSet { get; }

        #endregion
    }
}
