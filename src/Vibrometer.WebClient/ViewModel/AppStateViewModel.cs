using System.Collections.Generic;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;
using Vibrometer.WebClient.ViewModel;

namespace Vibrometer.WebClient.Model
{
    public class AppStateViewModel : BindableBase
    {
        #region Field

        AppState _model;
        FpgaData _fpgaData;
        bool _isConnected;

        #endregion

        #region Constructors

        public AppStateViewModel()
        {
            _model = new AppState();

            this.IsConnected = true;
            this.ChartSettingsMap = new Dictionary<ApiSource, ChartSettingsViewModel>();
            this.Summary = new SettingsSummary(this.FpgaSettings);
            this.FpgaData = new FpgaData();

            foreach (ChartSettings settings in _model.ChartSettingsSet)
            {
                ChartSettingsViewModel chartSettings;

                chartSettings = new ChartSettingsViewModel(settings);
                chartSettings.PropertyChanged += (sender, e) => base.RaisePropertyChanged(nameof(this.ChartSettingsMap));

                this.ChartSettingsMap.Add(settings.Source, chartSettings);
            }

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

                this.Summary = new SettingsSummary(this.FpgaSettings);
                base.RaisePropertyChanged(nameof(this.ChartSettingsMap));
                base.RaisePropertyChanged(nameof(this.FpgaSettings));
            }
        }

        #endregion
    }
}
