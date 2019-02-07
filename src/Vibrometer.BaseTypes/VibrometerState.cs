namespace Vibrometer.BaseTypes
{
    public struct VibrometerState
    {
        public int AS_Source { get; set; }
        public bool SG_FmEnabled { get; set; }
        public int SG_PhaseSignal { get; set; }
        public int SG_PhaseCarrier { get; set; }
        public bool DA_SwitchEnabled { get; set; }
        public int PT_LogScale { get; set; }
        public int PT_LogCountExtremum { get; set; }
        public int PT_ShiftExtremum { get; set; }
        public int FI_LogThrottle { get; set; }
        public bool FT_Enabled { get; set; }
        public int FT_LogCountAverages { get; set; }
        public int FT_LogThrottle { get; set; }
        public bool RW_Enabled { get; set; }
        public bool RW_RequestEnabled { get; set; }
        public int RW_LogLength { get; set; }
        public int RW_LogThrottle { get; set; }
        public int RW_ReadBuffer { get; set; }
    }
}
