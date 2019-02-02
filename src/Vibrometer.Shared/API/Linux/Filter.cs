using System;

namespace Vibrometer.Shared.API.Linux
{
    public class Filter : IFilter
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
                return ApiHelper.GetValue(ApiMethod.FI_LogThrottle, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.FI_LogThrottle, _address, value);
            }
        }
    }
}
