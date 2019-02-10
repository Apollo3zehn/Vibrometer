using System;

namespace Vibrometer.BaseTypes.API
{
    public class Filter
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public Filter(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public bool Enabled
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.FI_Enabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.FI_Enabled, _address, value ? 1U : 0U);
            }
        }

        public uint LogThrottle
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.FI_LogThrottle, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.FI_LogThrottle, _address, value);
            }
        }
    }
}
