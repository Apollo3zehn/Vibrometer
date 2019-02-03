namespace Vibrometer.Shared.API
{
    public interface ISignalGenerator
    {
        bool FmEnabled { get; set; }
        uint PhaseSignal { get; set; }
        uint PhaseCarrier { get; set; }
    }
}
