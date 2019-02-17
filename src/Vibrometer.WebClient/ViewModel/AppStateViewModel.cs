using System.Collections.Generic;
using System.Linq;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;
using Vibrometer.WebClient.ViewModel;

namespace Vibrometer.WebClient.Model
{
    public class AppStateViewModel : BindableBase
    {
        #region Field

        bool _isBitstreamLoaded;
        bool _isConnected;

        AppState _model;
        FpgaData _fpgaData;

        #endregion

        #region Constructors

        public AppStateViewModel()
        {
            _model = new AppState();

            this.IsConnected = true;
            this.Summary = new SettingsSummary(this.FpgaSettings);
            this.ChartSettingsMap = new Dictionary<ApiSource, ChartSettingsViewModel>();
            this.FpgaData = new FpgaData();

            this.InitializeChartSettingsMap();

            this.PageDescriptionSet = new List<PageDescription>()
            {
                new PageDescription("Home", "HO", "home", ""),
                new PageDescription("Analysis", "AN", "bar_chart", "analysis"),
                new PageDescription("Recording", "RC", "timeline", "recording"),
                new PageDescription("Settings", "SE", "settings", "settings")
            };
        }

        #endregion

        #region Properties

        public bool IsBitstreamLoaded
        {
            get { return _isBitstreamLoaded; }
            set { base.SetProperty(ref _isBitstreamLoaded, value); }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set { base.SetProperty(ref _isConnected, value); }
        }

        public FpgaSettings FpgaSettings
        {
            get
            {
                return _model.FpgaSettings;
            }
            set
            {
                this.Summary = new SettingsSummary(value);
                base.SetProperty(ref _model.FpgaSettings, value);
            }
        }

        public FpgaData FpgaData
        {
            get { return _fpgaData; }
            set { base.SetProperty(ref _fpgaData, value); }
        }

        public SettingsSummary Summary { get; private set; }
        public List<PageDescription> PageDescriptionSet { get; }
        public Dictionary<ApiSource, ChartSettingsViewModel> ChartSettingsMap { get; }

        public AppState Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;

                this.InitializeChartSettingsMap();
                this.Summary = new SettingsSummary(this.FpgaSettings);

                base.RaisePropertyChanged(nameof(this.ChartSettingsMap));
                base.RaisePropertyChanged(nameof(this.FpgaSettings));
            }
        }

        #endregion

        #region "Methods"

        private void InitializeChartSettingsMap()
        {
            this.ChartSettingsMap.ToList().ForEach(value =>
            {
                value.Value.PropertyChanged -= (sender, e) => base.RaisePropertyChanged(nameof(this.ChartSettingsMap));
            });

            this.ChartSettingsMap.Clear();

            foreach (ChartSettings settings in this.Model.ChartSettingsSet)
            {
                ChartSettingsViewModel chartSettings;

                chartSettings = new ChartSettingsViewModel(settings);
                chartSettings.PropertyChanged += (sender, e) => base.RaisePropertyChanged(nameof(this.ChartSettingsMap));

                this.ChartSettingsMap.Add((ApiSource)settings.Source, chartSettings);
            }
        }

        #endregion
    }
}
