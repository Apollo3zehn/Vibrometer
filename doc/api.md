#Name                Address / Description  Dir    Comment

Signal_Generator    0x4120_0000
    [0:0]           fm_enable               RW
    [5:1]           shift_carrier           RW
    [31:6]          phase_signal            RW
    + 0x0008
    [26:0]          phase_carrier           RW

Data_Acquisition    0x4121_0000
    [0:0]           switch                  RW

Position_Tracker    0x4122_0000
    [2:0]           shift_extremum          RW
    [7:3]           log_count_extremum      RW
    [12:8]          log_scale               RW
    + 0x0008 
    [31:0]          threshold               RO     31_[max, min]_0

Filter              0x4123_0000
    [0:0]           enable                  RW
    [5:1]           log_throttle            RW

Fourier_Transform   0x4124_0000
    [0:0]           enable                  RW
    [5:1]           log_count_averages      RW
    [10:6]          log_throttle            RW

RAM_Writer          0x4125_0000
    [0:0]           enable                  RW
    [1:1]           request                 RW
    [6:2]           log_length              RW
    [11:7]          log_throttle            RW
    + 0x0008
    [31:0]          read_buffer             RO
    [31:0]          address                 WO

AXIS_Switch         0x43C0_0000             RW    see AXI-4 Stream Switch documentation