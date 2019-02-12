using System;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.API
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
                return ApiProxy.GetValue(ApiParameter.FI_Enabled, _address) > 0;
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.FI_Enabled, _address, value ? 1U : 0U);
            }
        }

        public uint LogThrottle
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.FI_LogThrottle, _address);
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.FI_LogThrottle, _address, value);
            }
        }
    }
}
