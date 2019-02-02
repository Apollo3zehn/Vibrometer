using System;

namespace Vibrometer.Shared.API
{
    public static class ApiHelper
    {
        public static void SetValue(ApiMethod method, IntPtr address, uint value)
        {
            uint storage;
            uint max;
            ApiRecord record;
            IntPtr realAddress;

            record = SystemParameters.ApiInfo[method];
            realAddress = IntPtr.Add(address, record.Offset);
            max = (uint)(Math.Pow(2, record.Size) - 1);

            if (value < record.Min || value > record.Max)
            {
                throw new ArgumentException(nameof(value));
            }

            storage = ApiHelper.GetValue(0, 32, realAddress);
            storage &= ~(max << record.Shift);
            storage |= (value << record.Shift);

            unsafe
            {
                *(uint*)(realAddress.ToPointer()) = storage;
            }
        }

        public static uint GetValue(ApiMethod method, IntPtr address)
        {
            ApiRecord record;

            record = SystemParameters.ApiInfo[method];

            return ApiHelper.GetValue(record.Shift, record.Size, IntPtr.Add(address, record.Offset));
        }

        public static uint GetValue(int shift, int size, IntPtr address)
        {
            uint value;
            uint max;

            max = (uint)(Math.Pow(2, size) - 1);

            unsafe
            {
                value = *(uint*)(address.ToPointer());
            }

            return (value >> shift) & max;
        }
    }
}
