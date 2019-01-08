`timescale 1ns / 1ps

module axis_sniffer #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // signals
    input  wire                             aclk,
    output wire [AXIS_TDATA_WIDTH-1:0]      t_data,
    
    // axis slave
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    output wire                             S_AXIS_tready,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata
);          
 
    assign M_AXIS_tvalid                    = S_AXIS_tvalid;
    assign M_AXIS_tdata                     = S_AXIS_tdata;
    assign S_AXIS_tready                    = M_AXIS_tready;
    assign t_data                           = S_AXIS_tdata;

endmodule