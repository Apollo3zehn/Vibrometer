using System;
using System.Runtime.InteropServices;
using Vibrometer.Infrastructure;
using Vibrometer.Infrastructure.API;

namespace Vibrometer.API
{
    public static class ApiProxy
    {
        #region Fields

        private static IntPtr fakeArray;
        private const int FAKE_BUFFER_SIZE = 100;
        private const int SWITCH_OFFSET = 80; // width = 64 bytes => 64 / 4 => width = 12 * uint32
        private const int GPIO_OFFSET = 8;

        #endregion

        #region Constructors

        static ApiProxy()
        {
            ApiProxy.IsEnabled = true;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // TODO: free array
                fakeArray = Marshal.AllocHGlobal(ApiProxy.FAKE_BUFFER_SIZE * SystemParameters.BYTE_COUNT);

                unsafe
                {
                    new Span<uint>(fakeArray.ToPointer(), ApiProxy.FAKE_BUFFER_SIZE).Clear();
                }
            }
        }

        #endregion

        #region Properties

        public static bool IsEnabled { get; set; }

        #endregion

        #region Methods

        public static void SetValue(ApiParameter parameter, IntPtr address, uint value)
        {
            ApiRecord record;
            IntPtr realAddress;

            record = ApiInfo.Instance[parameter];
            realAddress = ApiProxy.GetAddress(record.Group, address);

            ApiProxy.InternalSetValue(record, IntPtr.Add(realAddress, record.Offset), value);
        }

        public static uint GetValue(ApiParameter parameter, IntPtr address)
        {
            ApiRecord record;
            IntPtr realAddress;

            record = ApiInfo.Instance[parameter];
            realAddress = ApiProxy.GetAddress(record.Group, address);

            return ApiProxy.InternalGetValue(record.Shift, record.Size, IntPtr.Add(realAddress, record.Offset));
        }

        private static void InternalSetValue(ApiRecord record, IntPtr address, uint value)
        {
            uint storage;
            uint max;

            if (!ApiProxy.IsEnabled)
            {
                return;
            }

            max = (uint)(Math.Pow(2, record.Size) - 1);

            if (value < record.Min || value > record.Max)
            {
                throw new ArgumentException(nameof(value));
            }

            storage = ApiProxy.InternalGetValue(0, 32, address);
            storage &= ~(max << record.Shift);
            storage |= (value << record.Shift);

            unsafe
            {
                *(uint*)(address.ToPointer()) = storage;
            }
        }

        private static uint InternalGetValue(int shift, int size, IntPtr address)
        {
            uint value;
            uint max;

            if (!ApiProxy.IsEnabled)
            {
                return 0;
            }

            max = (uint)(Math.Pow(2, size) - 1);

            unsafe
            {
                value = *(uint*)(address.ToPointer());
            }

            return (value >> shift) & max;
        }

        private static IntPtr GetAddress(ApiGroup group, IntPtr address)
        {
            IntPtr realAddress;

            realAddress = address;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (group == ApiGroup.AxisSwitch)
                {
                    realAddress = IntPtr.Add(fakeArray, ApiProxy.SWITCH_OFFSET * SystemParameters.BYTE_COUNT);
                }
                else
                {
                    // - # of ports     = 2
                    // - AXI GPIO width = 4 bytes       (SystemParameters.BYTE_COUNT)
                    // - GPIO offset    = 8 bytes       (ApiProxy.GPIO_OFFSET)
                    // ==============================================================
                    // total size per dual port GPIO = 4 * 2 + (8 - 4) = 8 + 4 = 12 bytes
                    realAddress = IntPtr.Add(fakeArray, (int)(group - 1) * (ApiProxy.GPIO_OFFSET + SystemParameters.BYTE_COUNT));
                }
            }

            return realAddress;
        }

        #endregion
    }
}
