using System;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.API
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
                return ApiProxy.GetValue(ApiParameter.RW_Enabled, _address) > 0;
            }
            set
            {
                if (value)
                {
                    this.Address = SystemParameters.DATA_BASE;
                }

                ApiProxy.SetValue(ApiParameter.RW_Enabled, _address, value ? 1U : 0U);
            }
        }

        public bool RequestEnabled
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.RW_RequestEnabled, _address) > 0;
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.RW_RequestEnabled, _address, value ? 1U : 0U);
            }
        }

        public uint LogLength
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.RW_LogLength, _address);
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.RW_LogLength, _address, value);
            }
        }

        public uint LogThrottle
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.RW_LogThrottle, _address);
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.RW_LogThrottle, _address, value);
            }
        }

        public uint Address
        {
            set
            {
                ApiProxy.SetValue(ApiParameter.RW_Address, _address, value);
            }
        }

        public uint ReadBuffer
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.RW_ReadBuffer, _address);
            }
        }
    }
}
