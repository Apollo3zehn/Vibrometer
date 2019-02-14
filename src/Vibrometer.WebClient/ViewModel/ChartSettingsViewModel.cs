using System;
using Vibrometer.Infrastructure.API;
using Vibrometer.WebClient.Model;

namespace Vibrometer.WebClient.ViewModel
{
    public class ChartSettingsViewModel : BindableBase
    {
        #region Fields

        private ChartSettings _model;
        private double _defaultYMin;
        private double _defaultYMax;

        #endregion

        #region Constructors

        public ChartSettingsViewModel(ChartSettings model)
        {
            _model = model;
            _defaultYMin = this.GetYMin();
            _defaultYMax = this.GetYMax();

            this.YMin = _defaultYMin;
            this.YMax = _defaultYMax;
        }

        #endregion

        #region Properties

        public double YMin
        {
            get
            {
                return _model.YMin;
            }
            set
            {
                if (value > this.YMax)
                {
                    base.SetProperty(ref _model.YMin, this.YMax);
                }
                else
                {
                    base.SetProperty(ref _model.YMin, value);
                }
            }
        }

        public double YMax
        {
            get
            {
                return _model.YMax;
            }
            set
            {
                if (value < this.YMin)
                {
                    base.SetProperty(ref _model.YMax, this.YMin);
                }
                else
                {
                    base.SetProperty(ref _model.YMax, value);
                }
            }
        }

        public ChartLimitMode LimitMode
        {
            get
            {
                return _model.LimitMode;
            }
            set
            {
                if (value != this.LimitMode)
                {
                    this.YMin = _defaultYMin;
                    this.YMax = _defaultYMax;
                }

                base.SetProperty(ref _model.LimitMode, value);
            }
        }

        public string YLabel
        {
            get { return this.GetYLabel(); }
        }

        public string Title
        {
            get { return this.GetTitle(); }
        }

        #endregion

        #region Methods

        private string GetTitle()
        {
            switch (_model.Source)
            {
                case ApiSource.NoSource:
                    return string.Empty;
                case ApiSource.Raw:
                    return "Raw";
                case ApiSource.Position:
                    return "Position";
                case ApiSource.Filter:
                    return "Filter";
                case ApiSource.FourierTransform:
                    return "Spectrum";
                default:
                    throw new ArgumentException();
            }
        }

        private string GetYLabel()
        {
            switch (_model.Source)
            {
                case ApiSource.NoSource:
                    return string.Empty;
                case ApiSource.Raw:
                    return "raw";
                case ApiSource.Position:
                    return "position";
                case ApiSource.Filter:
                    return "filtered position";
                case ApiSource.FourierTransform:
                    return "amplitude";
                default:
                    throw new ArgumentException();
            }
        }

        private double GetYMin()
        {
            switch (_model.Source)
            {
                case ApiSource.NoSource:
                    return 0;
                case ApiSource.Raw:
                    return -Math.Pow(2, 14 - 1);
                case ApiSource.Position:
                    return -Math.Pow(2, 32 - 1);
                case ApiSource.Filter:
                    return -Math.Pow(2, 16 - 1);
                case ApiSource.FourierTransform:
                    return 0;
                default:
                    throw new ArgumentException();
            }
        }

        private double GetYMax()
        {
            switch (_model.Source)
            {
                case ApiSource.NoSource:
                    return 0;
                case ApiSource.Raw:
                    return Math.Pow(2, 14 - 1) - 1;
                case ApiSource.Position:
                    return Math.Pow(2, 32 - 1) - 1;
                case ApiSource.Filter:
                    return Math.Pow(2, 16 - 1) - 1;
                case ApiSource.FourierTransform:
                    return Math.Pow(2, 16 - 1) - 1;
                default:
                    throw new ArgumentException();
            }
        }

        #endregion
    }
}
