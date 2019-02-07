using System;

namespace Vibrometer.BaseTypes.API
{
    public class FourierTransform
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public FourierTransform(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public bool Enabled
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.FT_Enabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.FT_Enabled, _address, value ? 1U : 0U);
            }
        }

        public uint LogCountAverages
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.FT_LogCountAverages, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.FT_LogCountAverages, _address, value);
            }
        }

        public uint LogThrottle
        {
            get
            {
                return ApiHelper.GetValue(ApiParameter.FT_LogThrottle, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiParameter.FT_LogThrottle, _address, value);
            }
        }
    }
}
