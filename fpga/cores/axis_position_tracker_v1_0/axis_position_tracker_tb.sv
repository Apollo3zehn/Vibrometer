`timescale 1ns / 1ps

module axis_position_tracker_tb #
(
    parameter integer                   S_AXIS_TDATA_WIDTH  = 32
);

    logic                               aclk                = 0;
    logic                               aresetn             = 0;
    logic [(S_AXIS_TDATA_WIDTH/2)-1:0]  lower_threshold     = -1;
    logic [(S_AXIS_TDATA_WIDTH/2)-1:0]  upper_threshold     = 1;
    logic [4:0]                         log_scale           = 0;
    logic                               S_AXIS_tvalid       = 1;
    logic [S_AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata;

    integer i                                               = 0;

    logic [(S_AXIS_TDATA_WIDTH/2)-1:0]  signal_a            = 0;
    logic [(S_AXIS_TDATA_WIDTH/2)-1:0]  signal_b            = 15;

    assign S_AXIS_tdata                 = {signal_b, signal_a};

    axis_position_tracker DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .lower_threshold(lower_threshold),
        .upper_threshold(upper_threshold),
        .log_scale(log_scale),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin

        repeat (2) @(posedge aclk);                     
            aresetn             = 1'b1;
        
        repeat (5) @(posedge aclk); 
            for (i=0; i<10; i=i+1) begin
                repeat (1) @(posedge aclk); 
                    signal_a = 15; 
                repeat (1) @(posedge aclk); 
                    signal_a = 12; 
                repeat (1) @(posedge aclk); 
                    signal_a = 9;
                repeat (1) @(posedge aclk); 
                    signal_a = 6;
                repeat (1) @(posedge aclk); 
                    signal_a = 3;
                repeat (1) @(posedge aclk); 
                    signal_a = 0;
                repeat (1) @(posedge aclk); 
                    signal_a = -3;
                repeat (1) @(posedge aclk); 
                    signal_a = -6;
                repeat (1) @(posedge aclk); 
                    signal_a = -9;
                repeat (1) @(posedge aclk); 
                    signal_a = -12;
                repeat (1) @(posedge aclk); 
                    signal_a = -15;
                repeat (1) @(posedge aclk); 
                    signal_a = -12;
                repeat (1) @(posedge aclk); 
                    signal_a = -9;
                repeat (1) @(posedge aclk); 
                    signal_a = -6;
                repeat (1) @(posedge aclk); 
                    signal_a = -3;
                repeat (1) @(posedge aclk); 
                    signal_a = 0;
                repeat (1) @(posedge aclk); 
                    signal_a = 3;
                repeat (1) @(posedge aclk); 
                    signal_a = 6;
                repeat (1) @(posedge aclk); 
                    signal_a = 9;
                repeat (1) @(posedge aclk); 
                    signal_a = 12;
            end

    end

    initial begin
        repeat (80) @(posedge aclk);                     
            signal_b = -15;
    end

    always 
        #4 aclk = ~aclk;

    initial begin
        #1500
        $finish;
    end

endmodule
