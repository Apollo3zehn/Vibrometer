namespace Vibrometer.Infrastructure.API
{
    public enum ApiSource : uint
    {
        NoSource = 0,
        Raw = 1,
        Position = 2,
        Filter = 3,
        FourierTransform = 4
    }
}
