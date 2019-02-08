using System.Collections.Generic;
using Vibrometer.BaseTypes;

namespace Vibrometer.WebClient.Model
{
    public class AppState : BindableBase
    {
        #region Field

        int[] _bufferContent;
        VibrometerState _vibrometerState;

        #endregion

        #region Constructors

        public AppState()
        {
            this.VibrometerState = new VibrometerState();

            this.PageDescriptionSet = new List<PageDescription>()
            {
                new PageDescription("Analysis", "AN", "show_chart", "analysis"),
                new PageDescription("Settings", "SE", "settings", "settings")
            };
        }

        #endregion

        #region Properties

        public int[] BufferContent
        {
            get { return _bufferContent; }
            set { base.SetProperty(ref _bufferContent, value); }
        }

        public VibrometerState VibrometerState
        {
            get
            {
                return _vibrometerState;
            }
            set
            {
                this.Summary = new SettingsSummary(value);
                base.SetProperty(ref _vibrometerState, value);
            }
        }

        public SettingsSummary Summary { get; private set; }
        public List<PageDescription> PageDescriptionSet { get; }

        #endregion
    }
}
