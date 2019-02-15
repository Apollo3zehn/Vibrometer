`timescale 1ns / 1ps

module fm_adapter #
(
    parameter integer                           CARRIER_PINC_WIDTH  = 32,
    parameter integer                           SIGNAL_PHASE_WIDTH  = 16,
    parameter integer                           S_AXIS_TDATA_WIDTH  = 16,
    parameter integer                           M_AXIS_TDATA_WIDTH  = 32
)
(
    // system signals
    input  wire                                 aclk,
    input  wire                                 aresetn,
    
    // IP signals
    input  wire                                 fm_enable,
    input  wire [4:0]                           shift_carrier,
    input  wire [CARRIER_PINC_WIDTH-1:0]        phase_carrier,
    output wire                                 switch_enable,
    
    // axis slave
    input  wire                                 S_AXIS_tvalid,
    input  wire [S_AXIS_TDATA_WIDTH-1:0]        S_AXIS_tdata,
    output wire                                 S_AXIS_tready,
    
    // axis master
    input  wire                                 M_AXIS_tready,
    output wire                                 M_AXIS_tvalid,
    output wire [M_AXIS_TDATA_WIDTH-1:0]        M_AXIS_tdata
);

    wire   [SIGNAL_PHASE_WIDTH-1:0]             tdata_unsigned;

    reg                                         switch, switch_next;
    reg    [SIGNAL_PHASE_WIDTH-1:0]             phase, phase_next;
    reg                                         slope, slope_next;

    assign tdata_unsigned                       = S_AXIS_tdata + (1 << (SIGNAL_PHASE_WIDTH - 1));
    assign S_AXIS_tready                        = M_AXIS_tready;
    assign M_AXIS_tvalid                        = fm_enable ? S_AXIS_tvalid : 1'b1;
    assign M_AXIS_tdata                         = fm_enable ? (tdata_unsigned >> shift_carrier) : phase_carrier;
    assign switch_enable                        = switch;

    always @(posedge aclk) begin
        if (~aresetn) begin
            switch          <= 1'b0;
            phase           <= 0;
            slope           <= 1'b0;
        end else begin
            switch          <= switch_next;
            phase           <= phase_next;
            slope           <= slope_next;
        end 
    end
      
    always @* begin
        switch_next          = switch;
        phase_next           = tdata_unsigned;
        slope_next           = slope;

        if (fm_enable) begin
            if (tdata_unsigned > phase)
                slope_next   = 1'b1;
            else if (tdata_unsigned < phase)
                slope_next   = 1'b0;

            if (slope == 1'b0 && slope_next == 1'b1)
                switch_next  = ~switch;
        end else begin
            switch_next      = 1'b0;
        end
    end

endmodule