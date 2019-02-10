`timescale 1ns / 1ps

module axis_extremum_finder_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         aclk                = 0;
    reg                         aresetn             = 0;
    reg [4:0]                   log_count           = 0;
    reg [2:0]                   shift               = 0;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata        = 0;
    reg                         S_AXIS_tvalid       = 1;
 
    axis_extremum_finder DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .log_count(log_count),
        .shift(shift),
        .S_AXIS_tdata(S_AXIS_tdata),
        .S_AXIS_tvalid(S_AXIS_tvalid)
    );
     
    initial begin
        repeat (1) @(posedge aclk); 
            S_AXIS_tdata    = -20;
            aresetn         = 1;

        repeat (1) @(posedge aclk);       
            S_AXIS_tdata    = -10;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 10;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 20;

        repeat (1) @(posedge aclk);        
            S_AXIS_tdata    = 10;
        
        // start
        repeat (1) @(posedge aclk);
            log_count    = 3;
            S_AXIS_tdata    = -10;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = -30;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = -40;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = -20;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 10;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 20;       
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 30;
                
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 40;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 50;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 60;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 50;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    = 40;
    end

    always 
        #4 aclk = ~aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
