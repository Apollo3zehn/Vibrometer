using System;

namespace Vibrometer.Shared.API
{
    public class RamWriter
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public RamWriter(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public bool Enabled
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.RW_Enabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.RW_Enabled, _address, value ? 1U : 0U);
            }
        }

        public bool RequestEnabled
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.RW_RequestEnabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.RW_RequestEnabled, _address, value ? 1U : 0U);
            }
        }

        public uint LogLength
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.RW_LogLength, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.RW_LogLength, _address, value);
            }
        }

        public uint LogThrottle
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.RW_LogThrottle, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.RW_LogThrottle, _address, value);
            }
        }

        public uint Address
        {
            set
            {
                ApiHelper.SetValue(ApiMethod.RW_Address, _address, value);
            }
        }

        public uint ReadBuffer
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.RW_ReadBuffer, _address);
            }
        }
    }
}
