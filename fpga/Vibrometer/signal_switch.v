`timescale 1ns / 1ps

module signal_switch #
(
    parameter integer                       DATA_WIDTH      = 16
)
(
    // system signals
    input  wire                             SYS_aclk,
    
    // FC signals
    input  wire                             switch,
    input  wire [DATA_WIDTH-1:0]            a,
    input  wire [DATA_WIDTH-1:0]            b,
    output wire [DATA_WIDTH-1:0]            x,
    output wire [DATA_WIDTH-1:0]            y
);

    assign x                                = switch ? b : a;
    assign y                                = switch ? a : b;

endmodule