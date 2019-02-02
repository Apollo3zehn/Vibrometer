﻿using System;

namespace Vibrometer.Shared.API.Linux
{
    public class FourierTransform : IFourierTransform
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
                return ApiHelper.GetValue(ApiMethod.FT_Enabled, _address) > 0;
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.FT_Enabled, _address, value ? 1U : 0U);
            }
        }

        public uint LogCountAverages
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.FT_LogCountAverages, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.FT_LogCountAverages, _address, value);
            }
        }

        public uint LogThrottle
        {
            get
            {
                return ApiHelper.GetValue(ApiMethod.FT_LogThrottle, _address);
            }
            set
            {
                ApiHelper.SetValue(ApiMethod.FT_LogThrottle, _address, value);
            }
        }
    }
}
