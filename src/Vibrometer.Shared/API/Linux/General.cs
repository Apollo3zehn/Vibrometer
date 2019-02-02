using System;

namespace Vibrometer.Shared.API.Linux
{
    public class General : IGeneral
    {
        #region Fields

        private IntPtr _address;

        #endregion

        #region Constructors

        public General(IntPtr address)
        {
            _address = address;
        }

        #endregion

        public Source Source
        {
            get
            {
                uint value;

                value = ApiHelper.GetValue(ApiMethod.GE_Source, _address + 0x0040);

                // return 0, if switch is disabled
                if (value >= 0x80000000)
                {
                    return 0;
                }
                else
                {
                    return (Source)(value + 1);
                }
            }
            set
            {
                if (value == 0)
                {
                    // disable switch
                    ApiHelper.SetValue(ApiMethod.GE_Source, _address + 0x0040, 0x8000_0000);
                }
                else if (value <= Source.FourierTransform)
                {
                    // connect slave[value - 1] with master[0]
                    ApiHelper.SetValue(ApiMethod.GE_Source, _address + 0x0040, (uint)value - 1);
                }
                else
                {
                    throw new ArgumentException();
                }

                // commit settings
                ApiHelper.SetValue(ApiMethod.GE_Source, _address + 0x0000, 0x0000_0002);
            }
        }
    }
}
