using System;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.API
{
    public class PositionTracker
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public PositionTracker(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public uint LogScale
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.PT_LogScale, _address);
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.PT_LogScale, _address, value);
            }
        }

        public uint LogCountExtremum
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.PT_LogCountExtremum, _address);
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.PT_LogCountExtremum, _address, value);
            }
        }

        public uint ShiftExtremum
        {
            get
            {
                return ApiProxy.GetValue(ApiParameter.PT_ShiftExtremum, _address);
            }
            set
            {
                ApiProxy.SetValue(ApiParameter.PT_ShiftExtremum, _address, value);
            }
        }

        public (short, short) Threshold
        {
            get
            {
                short a;
                short b;
                uint value;

                value = ApiProxy.GetValue(ApiParameter.PT_Threshold, _address);

                a = unchecked((short)(value & ~0xFFFF0000));
                b = unchecked((short)(value >> 16));

                return (a, b);
            }
        }
    }
}
