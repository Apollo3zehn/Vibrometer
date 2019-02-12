`timescale 1ns / 1ps

module axis_data_switch #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                             aclk,
    
    // IP signals
    input  wire                             switch,

    // slave
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    input  wire                             S_AXIS_tvalid,
    output wire                             S_AXIS_tready,

    // master
    input  wire                             M_AXIS_tready,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata,
    output wire                             M_AXIS_tvalid
);

    assign M_AXIS_tvalid                    = S_AXIS_tvalid;
    assign S_AXIS_tready                    = M_AXIS_tready;

    assign M_AXIS_tdata                     = switch ? S_AXIS_tdata : {S_AXIS_tdata[(AXIS_TDATA_WIDTH/2)-1:0], S_AXIS_tdata[AXIS_TDATA_WIDTH-1:AXIS_TDATA_WIDTH/2]};

endmodule