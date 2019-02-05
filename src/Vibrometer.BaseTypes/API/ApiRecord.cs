using System;

namespace Vibrometer.BaseTypes.API
{
    public class ApiRecord
    {
        public ApiMethod Method { get; }
        public ApiGroup Group { get; }
        public string DisplayName { get; }
        public int Offset { get; }
        public int Shift { get; }
        public int Size { get; }
        public uint Min { get; }
        public uint Max { get; }

        public ApiRecord(ApiMethod method, ApiGroup group, string displayName, int offset, int shift, int size)
        {
            this.Method = method;
            this.Group = group;
            this.DisplayName = displayName;
            this.Offset = offset;
            this.Shift = shift;
            this.Size = size;
            this.Min = 0;
            this.Max = (uint)(Math.Pow(2, size) - 1);
        }

        public ApiRecord(ApiMethod method, ApiGroup group, string displayName, int offset, int shift, int size, uint min, uint max)
        {
            this.Method = method;
            this.Group = group;
            this.DisplayName = displayName;
            this.Offset = offset;
            this.Shift = shift;
            this.Size = size;
            this.Min = min;
            this.Max = max;
        }
    }
}
