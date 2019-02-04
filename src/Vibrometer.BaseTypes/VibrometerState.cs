using Vibrometer.Shared.API;

namespace Vibrometer.BaseTypes
{
    public struct VibrometerState
    {
        public ApiSource AS_Source { get; set; }
        public bool SG_FmEnabled { get; set; }
        public uint SG_PhaseSignal { get; set; }
        public uint SG_PhaseCarrier { get; set; }
        public bool DA_SwitchEnabled { get; set; }
        public uint PT_LogScale { get; set; }
        public uint PT_LogCountExtremum { get; set; }
        public uint PT_ShiftExtremum { get; set; }
        public uint FI_LogThrottle { get; set; }
        public bool FT_Enabled { get; set; }
        public uint FT_LogCountAverages { get; set; }
        public uint FT_LogThrottle { get; set; }
        public bool RW_Enabled { get; set; }
        public bool RW_RequestEnabled { get; set; }
        public uint RW_LogLength { get; set; }
        public uint RW_LogThrottle { get; set; }
        public uint RW_ReadBuffer { get; set; }
    }
}
