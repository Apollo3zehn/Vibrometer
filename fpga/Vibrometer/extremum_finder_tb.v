`timescale 1ns / 1ps

module extremum_finder_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         SYS_aclk;
    reg                         SYS_aresetn;
    reg [31:0]                  EF_log_count;
    reg [5:0]                   EF_log_shift;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata;
    reg                         S_AXIS_tvalid;
 
    extremum_finder efinder (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .EF_log_count(EF_log_count),
        .EF_log_shift(EF_log_shift),
        .S_AXIS_tdata(S_AXIS_tdata),
        .S_AXIS_tvalid(S_AXIS_tvalid)
    );
     
    initial begin
        SYS_aclk = 0;
        SYS_aresetn = 0;
        EF_log_count = 3;
        EF_log_shift = 3;
        S_AXIS_tvalid = 1;

        // 1        
        S_AXIS_tdata = 0;
        SYS_aresetn = 0;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
                    
        S_AXIS_tdata = -20;
        SYS_aresetn = 1;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = -10;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = 10;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;

        S_AXIS_tdata = 20;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;        
        
        S_AXIS_tdata = 10;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = -10;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = -30;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = -20;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = -10;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = 10;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        S_AXIS_tdata = 20;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        forever #1 SYS_aclk = ~SYS_aclk;
    end

endmodule
