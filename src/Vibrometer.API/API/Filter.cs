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
