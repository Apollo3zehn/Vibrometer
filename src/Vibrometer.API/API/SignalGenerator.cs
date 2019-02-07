using System;

namespace Vibrometer.BaseTypes.API
{
    public class SignalGenerator
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public SignalGenerator(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public bool FmEnabled
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.SG_FmEnabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.SG_FmEnabled, _address, value ? 1U : 0U);
            }
        }

        public uint PhaseSignal
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.SG_PhaseSignal, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.SG_PhaseSignal, _address, value);
            }
        }

        public uint PhaseCarrier
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.SG_PhaseCarrier, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.SG_PhaseCarrier, _address, value);
            }
        }
    }
}
