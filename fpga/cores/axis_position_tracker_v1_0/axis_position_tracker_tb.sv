`timescale 1ns / 1ps

module axis_position_tracker_tb #
(
    parameter integer             AXIS_TDATA_WIDTH    = 32
);

    logic                         aclk                = 0;
    logic                         aresetn             = 0;
    logic [AXIS_TDATA_WIDTH-1:0]  lower_treshold      = -10;
    logic [AXIS_TDATA_WIDTH-1:0]  upper_treshold      = 10;
    logic                         S_AXIS_tvalid       = 1;
    logic [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata        = 0;

    integer i                                       = 0;

    axis_position_tracker DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .lower_treshold(lower_treshold),
        .upper_treshold(upper_treshold),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin

        repeat (1) @(posedge aclk);                     
            S_AXIS_tdata        = -5;
            aresetn             = 1;
        
        repeat (1) @(posedge aclk); 
            for (i=0; i<6; i=i+1) begin
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 10; 
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 5;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 0;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = -5;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = -10;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = -15;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = -10;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = -5;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 0;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 5;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 10;
                repeat (1) @(posedge aclk); 
                    S_AXIS_tdata = 15;
            end

    end

    always 
        #4 aclk = ~aclk;

    initial begin
        #400
        $finish;
    end

endmodule
