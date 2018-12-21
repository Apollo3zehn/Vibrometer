`timescale 1ns / 1ps

module sign_finder_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         SYS_aclk;
    reg                         SYS_aresetn;
    reg [4:0]                   SF_log_count;
    reg                         S_AXIS_tvalid;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata;
 
    sign_finder sfinder (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .SF_log_count(SF_log_count),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        SYS_aclk = 0;
        SYS_aresetn = 0;
        SF_log_count = 3;
        S_AXIS_tvalid = 1'b0;

        S_AXIS_tdata = 0;
        SYS_aresetn = 0;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
                    
        S_AXIS_tdata = -5;
        SYS_aresetn = 1;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;

        //   
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 19; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 27; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 35; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 42; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 48; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 53; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 57; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 60; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 62; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 63; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 62; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 60; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 57; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 53; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 48; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 42; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 35; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 27; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 19; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -19; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -27; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -35; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -42; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -48; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -53; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -57; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -60; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -62; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -63; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -62; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -60; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -57; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -53; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -48; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -42; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -35; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -27; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -19; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
                
        forever #1 SYS_aclk = ~SYS_aclk;
    end

endmodule
