`timescale 1ns / 1ps

module fm_manager #
(
    parameter integer                           CARRIER_PINC_WIDTH  = 32,
    parameter integer                           SIGNAL_PHASE_WIDTH  = 16,
    parameter integer                           S_AXIS_TDATA_WIDTH  = 16,
    parameter integer                           M_AXIS_TDATA_WIDTH  = 32
)
(
    // system signals
    input  wire                                 aclk,
    
    // FM signals
    input  wire                                 fm_enable,
    input  wire [CARRIER_PINC_WIDTH-1:0]        phase_carrier,
    
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

    assign tdata_unsigned                       = S_AXIS_tdata + (2 << (SIGNAL_PHASE_WIDTH - 1));
    assign S_AXIS_tready                        = M_AXIS_tready;
    assign M_AXIS_tvalid                        = fm_enable ? S_AXIS_tvalid : 1'b1;
    assign M_AXIS_tdata                         = fm_enable ? tdata_unsigned : phase_carrier;

endmodule