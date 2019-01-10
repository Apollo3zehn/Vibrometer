`timescale 1ns / 1ps

module axis_throttler_tb #
(
     parameter integer              AXIS_TDATA_WIDTH    = 32
);

    reg                             aclk;
    reg                             aresetn;
    reg [4:0]                       log_throttle;
    
    reg                             M_AXIS_tready;
    reg                             S_AXIS_tvalid;
    reg [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata;

    axis_throttler athrottler (
        .aclk(aclk),
        .aresetn(aresetn),
        .log_throttle(log_throttle),
        .M_AXIS_tready(M_AXIS_tready),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        aclk            = 0;
        aresetn         = 0;
        log_throttle    = 3;
        M_AXIS_tready   = 1;
        S_AXIS_tvalid   = 0;
        S_AXIS_tdata    = 0;

        #16;
        aresetn         = 1;
        S_AXIS_tvalid   = 1;
        S_AXIS_tdata    = 2;

        #80;
        S_AXIS_tvalid   = 0;
    end

    always 
        #4 aclk = !aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
