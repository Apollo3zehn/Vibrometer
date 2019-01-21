`timescale 1ns / 1ps

module axis_throttler_tb #
(
     parameter integer              AXIS_TDATA_WIDTH    = 32
);

    reg                             aclk                = 0;
    reg                             aresetn             = 0;
    reg [4:0]                       log_throttle        = 3;
    
    reg                             M_AXIS_tready       = 1;
    reg                             S_AXIS_tvalid       = 0;
    reg [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata        = 0;

    axis_throttler DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .log_throttle(log_throttle),
        .M_AXIS_tready(M_AXIS_tready),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        repeat (2) @(posedge aclk);
            aresetn         <= 1;
            S_AXIS_tvalid   <= 1;
            S_AXIS_tdata    <= 2;

        repeat (10) @(posedge aclk);
            S_AXIS_tvalid   <= 0;
    end

    always 
        #4 aclk = ~aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
