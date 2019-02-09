namespace Vibrometer.WebClient.Model
{
    public struct ChartPoint
    {
        public double x { get; set; }
        public double y { get; set; }

        public ChartPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
