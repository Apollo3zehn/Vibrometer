using System;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.API
{
    public class AxisSwitch
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public AxisSwitch(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public ApiSource Source
        {
            get
            {
                uint value;

                value = ApiProxy.GetValue(ApiParameter.AS_Source, _address);

                // return 0, if switch is disabled
                if (value >= 0x80000000)
                {
                    return 0;
                }
                else
                {
                    return (ApiSource)(value + 1);
                }
            }
            set
            {
                if (value == 0)
                {
                    // disable switch
                    ApiProxy.SetValue(ApiParameter.AS_Source, _address, 0x8000_0000);
                }
                else if (value <= ApiSource.FourierTransform)
                {
                    // connect slave[value - 1] with master[0]
                    ApiProxy.SetValue(ApiParameter.AS_Source, _address, (uint)value - 1);
                }
                else
                {
                    throw new ArgumentException();
                }

                // commit settings
                ApiProxy.SetValue(ApiParameter.AS_Commit, _address, 0x0000_0002);
            }
        }
    }
}
