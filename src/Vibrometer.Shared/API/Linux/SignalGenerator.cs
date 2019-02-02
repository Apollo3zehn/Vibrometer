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

        public uint Phase
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.SG_Phase, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.SG_Phase, _address, value);
            }
        }
    }
}
