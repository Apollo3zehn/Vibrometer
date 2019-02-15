`timescale 1ns / 1ps

module fm_adapter_tb #
(
     parameter integer              CARRIER_PINC_WIDTH  = 32,
     parameter integer              AXIS_TDATA_WIDTH    = 32
);

    reg                             aclk                = 0;
    reg                             aresetn             = 0;

    reg                             fm_enable           = 1;
    reg [4:0]                       shift_carrier       = 0;
    reg [CARRIER_PINC_WIDTH-1:0]    phase_carrier       = 0;
    
    reg signed [AXIS_TDATA_WIDTH-1:0] S_AXIS_tdata      = 0;

    fm_adapter DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .fm_enable(fm_enable),
        .shift_carrier(shift_carrier),
        .phase_carrier(phase_carrier),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        repeat (2) @(posedge aclk);
            aresetn         <= 1;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 1;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 2;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 3;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 2;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 1;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 0;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= -1;
        
        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= -2;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= -3;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= -2;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= -1;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 0;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 1;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 2;

        repeat (1) @(posedge aclk);
            S_AXIS_tdata    <= 3;
    end

    always 
        #4 aclk = ~aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
