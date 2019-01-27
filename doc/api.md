#Name                Address / Description   Dir    Comment

Signal_Generator    0x4120_0000
    [31:0]          phase                   RW

Data_Acquisition    0x4121_0000
    [0:0]           switch                  RW

Position_Tracker    0x4122_0000
    [12:8]          log_scale               RW
    [7:3]           log_count_extremum      RW
    [2:0]           shift_extremum          RW
    + 0x0008 
    [31:0]          threshold               RO      31_[max, min]_0

Filter              0x4123_0000
    [4:0]           log_throttle            RW

Fourier_Transform   0x4124_0000
    [10:6]          log_throttle            RW      
    [5:1]           log_count_averages      RW
    [0:0]           enable                  RW      

RAM_Writer          0x4125_0000
    [11:7]          log_throttle            RW
    [6:2]           log_length              RW
    [1:1]           request                 RW
    [0:0]           enable                  RW
    + 0x0008
    [31:0]          read_buffer             RO
    [31:0]          address                 WO

AXIS_Switch         0x43C0_0000                     see AXI4-Stream Switch documentation