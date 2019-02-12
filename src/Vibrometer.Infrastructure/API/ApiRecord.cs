using System;

namespace Vibrometer.Infrastructure.API
{
    public class ApiRecord
    {
        public ApiParameter Parameter { get; }
        public ApiGroup Group { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public int Offset { get; }
        public int Shift { get; }
        public int Size { get; }
        public uint Min { get; }
        public uint Max { get; }

        public ApiRecord(ApiParameter parameter, ApiGroup group, string displayName, string description, int offset, int shift, int size)
        {
            this.Parameter = parameter;
            this.Group = group;
            this.DisplayName = displayName;
            this.Description = description;
            this.Offset = offset;
            this.Shift = shift;
            this.Size = size;
            this.Min = 0;
            this.Max = (uint)(Math.Pow(2, size) - 1);
        }

        public ApiRecord(ApiParameter parameter, ApiGroup group, string displayName, string description, int offset, int shift, int size, uint min, uint max)
        {
            this.Parameter = parameter;
            this.Group = group;
            this.DisplayName = displayName;
            this.Description = description;
            this.Offset = offset;
            this.Shift = shift;
            this.Size = size;
            this.Min = min;
            this.Max = max;
        }
    }
}
