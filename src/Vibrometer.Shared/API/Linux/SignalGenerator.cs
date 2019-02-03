using System;

namespace Vibrometer.Shared.API.Linux
{
    public class SignalGenerator : ISignalGenerator
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
                return ApiHelper.GetValue(ApiMethod.SG_FmEnabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.SG_FmEnabled, _address, value ? 1U : 0U);
            }
        }

        public uint PhaseSignal
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.SG_PhaseSignal, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.SG_PhaseSignal, _address, value);
            }
        }

        public uint PhaseCarrier
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.SG_PhaseCarrier, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.SG_PhaseCarrier, _address, value);
            }
        }
    }
}
