using System.Runtime.Serialization;

namespace Vibrometer.BaseTypes
{
    [DataContract]
    public struct VibrometerState
    {
        [DataMember(Name = "aS_Source")]
        public int AS_Source { get; set; }
        [DataMember(Name = "sG_FmEnabled")]
        public bool SG_FmEnabled { get; set; }
        [DataMember(Name = "sG_PhaseSignal")]
        public int SG_PhaseSignal { get; set; }
        [DataMember(Name = "sG_PhaseCarrier")]
        public int SG_PhaseCarrier { get; set; }
        [DataMember(Name = "dA_SwitchEnabled")]
        public bool DA_SwitchEnabled { get; set; }
        [DataMember(Name = "pT_LogScale")]
        public int PT_LogScale { get; set; }
        [DataMember(Name = "pT_LogCountExtremum")]
        public int PT_LogCountExtremum { get; set; }
        [DataMember(Name = "pT_ShiftExtremum")]
        public int PT_ShiftExtremum { get; set; }
        [DataMember(Name = "fI_LogThrottle")]
        public int FI_LogThrottle { get; set; }
        [DataMember(Name = "fT_Enabled")]
        public bool FT_Enabled { get; set; }
        [DataMember(Name = "fT_LogCountAverages")]
        public int FT_LogCountAverages { get; set; }
        [DataMember(Name = "fT_LogThrottle")]
        public int FT_LogThrottle { get; set; }
        [DataMember(Name = "rW_Enabled")]
        public bool RW_Enabled { get; set; }
        [DataMember(Name = "rW_LogLength")]
        public int RW_LogLength { get; set; }
        [DataMember(Name = "rW_LogThrottle")]
        public int RW_LogThrottle { get; set; }
    }
}
