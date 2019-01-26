`timescale 1ns / 1ps

module axis_position_tracker_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         aclk                = 0;
    reg                         aresetn             = 0;
    reg [AXIS_TDATA_WIDTH-1:0]  FC_lower_treshold   = -10;
    reg [AXIS_TDATA_WIDTH-1:0]  FC_upper_treshold   = 10;
    reg                         S_AXIS_tvalid       = 1;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata        = 0;

    integer i                                       = 0;

    axis_position_tracker DUT (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .FC_lower_treshold(FC_lower_treshold),
        .FC_upper_treshold(FC_upper_treshold),
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
