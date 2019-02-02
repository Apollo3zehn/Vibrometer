namespace Vibrometer.Shared.API
{
    public interface IRamWriter
    {
        bool Enabled { get; set; }
        bool RequestEnabled { get; set; }
        uint LogLength { get; set; }
        uint LogThrottle { get; set; }
        uint Address { set; }
        uint ReadBuffer { get; }
    }
}
