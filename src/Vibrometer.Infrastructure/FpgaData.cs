using System;

namespace Vibrometer.Infrastructure
{
    public struct FpgaData
    {
        public FpgaData(int lowerThreshold, int upperThreshold, Memory<int> buffer)
        {
            this.LowerThreshold = lowerThreshold;
            this.UpperThreshold = upperThreshold;
            this.Buffer = buffer;
        }

        public int LowerThreshold { get; set; }
        public int UpperThreshold { get; set; }
        public Memory<int> Buffer { get; set; }
    }
}
