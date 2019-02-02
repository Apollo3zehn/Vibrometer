namespace Vibrometer.Shared.API
{
    public interface IPositionTracker
    {
        uint LogScale { get; set; }
        uint LogCountExtremum { get; set; }
        uint ShiftExtremum { get; set; }
        (short, short) Threshold { get; }
    }
}
