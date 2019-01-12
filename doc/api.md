#Name                Address / Description   Dir    Comment

General             0x4120_0000
    [31:0]          position                RO

Signal_Generator    0x4121_0000
    [31:0]          phase                   RW

Data_Acquisition    0x4122_0000
    [0:0]           switch                  RW
    + 0x0008
    [31:0]          raw                     RO      31_[ch. b, ch.a]_0

Position_Tracker    0x4123_0000
    [7:3]           log_count_extremum      RW
    [2:0]           shift_extremum          RW
    + 0x0008
    [31:0]          threshold               RO      31_[max, min]_0

RAM_Writer          0x4124_0000
    [11:7]          log_throttle            RW      
    [6:2]           log_length              RW
    [1:1]           request                 RW
    [0:0]           enable                  RW
    + 0x0008
    [31:0]          read_buffer             RO
    [31:0]          address                 WO