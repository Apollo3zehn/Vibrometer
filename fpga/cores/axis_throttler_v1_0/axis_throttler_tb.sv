`timescale 1ns / 1ps

module axis_throttler_tb #
(
     parameter integer                AXIS_TDATA_WIDTH    = 32
);

    logic                             aclk                = 0;
    logic                             aresetn             = 0;
    logic [4:0]                       log_throttle        = 3;
    
    logic                             M_AXIS_tready       = 1;
    logic                             S_AXIS_tvalid       = 0;
    logic [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata        = 0;

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
