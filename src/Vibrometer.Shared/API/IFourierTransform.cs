namespace Vibrometer.Shared.API
{
    public interface IFourierTransform
    {
        bool Enabled { get; set; }
        uint LogCountAverages { get; set; }
        uint LogThrottle { get; set; }
    }
}
