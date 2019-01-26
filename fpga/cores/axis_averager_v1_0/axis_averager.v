`timescale 1ns / 1ps

module axis_averager #
(
    parameter integer                       AXIS_TDATA_WIDTH     = 32,
    parameter integer                       BRAM_DATA_WIDTH     = 32,
    parameter integer                       BRAM_ADDR_WIDTH     = 16,
    parameter integer                       AVERAGES_WIDTH      = 32
)
(
    // System signals
    input  wire                             aclk,
    input  wire                             aresetn,
    
    // Slave side
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    input  wire                             S_AXIS_tvalid,
    output wire                             S_AXIS_tready,

    // Master side
    input  wire                             M_AXIS_tready,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata,
    output wire                             M_AXIS_tvalid,

    // BRAM PORT A
    input  wire [BRAM_DATA_WIDTH-1:0]       bram_porta_rddata,
    output wire                             bram_porta_clk,
    output wire                             bram_porta_rst,
    output wire [BRAM_ADDR_WIDTH-1:0]       bram_porta_addr,
    output wire [BRAM_DATA_WIDTH-1:0]       bram_porta_wrdata,
    output wire                             bram_porta_we,
    
    // BRAM PORT B
    input  wire [BRAM_DATA_WIDTH-1:0]       bram_portb_rddata,
    output wire                             bram_portb_clk,
    output wire                             bram_portb_rst,
    output wire [BRAM_ADDR_WIDTH-1:0]       bram_portb_addr,
    output wire [BRAM_DATA_WIDTH-1:0]       bram_portb_wrdata,
    output wire                             bram_portb_we
);

    assign M_AXIS_tdata = S_AXIS_tdata;
    assign M_AXIS_tvalid = S_AXIS_tvalid;
    assign S_AXIS_tready = M_AXIS_tready;

endmodule