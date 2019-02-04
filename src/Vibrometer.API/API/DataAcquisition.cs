using System;

namespace Vibrometer.Shared.API
{
    public class DataAcquisition
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public DataAcquisition(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public bool SwitchEnabled
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.DA_SwitchEnabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.DA_SwitchEnabled, _address, value ? 1U : 0U);
            }
        }
    }
}
