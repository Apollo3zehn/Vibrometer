namespace Vibrometer.Infrastructure
{
    public struct FpgaData
    {
        public FpgaData(int lowerThreshold, int upperThreshold, int[] buffer)
        {
            this.LowerThreshold = lowerThreshold;
            this.UpperThreshold = upperThreshold;
            this.Buffer = buffer;
        }

        public int LowerThreshold { get; set; }
        public int UpperThreshold { get; set; }
        public int[] Buffer { get; set; }
    }
}
