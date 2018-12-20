`timescale 1ns / 1ps

module frequency_counter #
(
    parameter integer                   AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                         SYS_aclk,
    input  wire                         SYS_aresetn,
    
    // FC signals
    input  wire[32:0]                   FC_averages_count,
    input  wire[32:0]                   FC_upper_treshold,
    input  wire[32:0]                   FC_lower_treshold,
    
    // axis slave
    input  wire                         S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata,
    output wire                         S_AXIS_tready,
    
    // axis master
    output wire                         M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]  M_AXIS_tdata
);

    localparam                          reset           = 3'b000, 
                                        clear_bram      = 3'b001, 
                                        wait_trigger    = 3'b010,
                                        measure         = 3'b011,
                                        finish          = 3'b100;

    reg         [AXIS_TDATA_WIDTH-1:0]  max;
    reg         [AXIS_TDATA_WIDTH-1:0]  min;

    assign S_AXIS_tready                = 1'b1;




endmodule
