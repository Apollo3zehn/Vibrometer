﻿using System;
using System.Runtime.InteropServices;

namespace Vibrometer.Shared.API
{
    public static class ApiHelper
    {
        private static IntPtr fakeArray;
        private const int FAKE_BUFFER_SIZE = 100;
        private const int SWITCH_OFFSET = 80; // width = 64 byte => 64 / 4 => width = 12

        static ApiHelper()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fakeArray = Marshal.AllocHGlobal(ApiHelper.FAKE_BUFFER_SIZE * SystemParameters.BYTE_COUNT);

                unsafe
                {
                    new Span<uint>(fakeArray.ToPointer(), ApiHelper.FAKE_BUFFER_SIZE).Clear();
                }
            }
        }

        public static void SetValue(ApiMethod method, IntPtr address, uint value)
        {
            ApiRecord record;
            IntPtr realAddress;

            record = ApiInfo.Instance[method];
            realAddress = ApiHelper.GetAddress(record.Group, address);

            ApiHelper.SetValueInternal(record, IntPtr.Add(realAddress, record.Offset), value);
        }

        public static uint GetValue(ApiMethod method, IntPtr address)
        {
            ApiRecord record;
            IntPtr realAddress;

            record = ApiInfo.Instance[method];
            realAddress = ApiHelper.GetAddress(record.Group, address);

            return ApiHelper.GetValueInternal(record.Shift, record.Size, IntPtr.Add(realAddress, record.Offset));
        }

        private static void SetValueInternal(ApiRecord record, IntPtr address, uint value)
        {
            uint storage;
            uint max;

            max = (uint)(Math.Pow(2, record.Size) - 1);

            if (value < record.Min || value > record.Max)
            {
                throw new ArgumentException(nameof(value));
            }

            storage = ApiHelper.GetValueInternal(0, 32, address);
            storage &= ~(max << record.Shift);
            storage |= (value << record.Shift);

            unsafe
            {
                *(uint*)(address.ToPointer()) = storage;
            }
        }

        private static uint GetValueInternal(int shift, int size, IntPtr address)
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

        private static IntPtr GetAddress(ApiGroup group, IntPtr address)
        {
            IntPtr realAddress;

            realAddress = address;

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (group == ApiGroup.AxisSwitch)
                {
                    realAddress = IntPtr.Add(fakeArray, ApiHelper.SWITCH_OFFSET * SystemParameters.BYTE_COUNT);
                }
                else
                {
                    realAddress = IntPtr.Add(fakeArray, (int)(group - 1) * SystemParameters.BYTE_COUNT);
                }
            }

            return realAddress;
        }
    }
}