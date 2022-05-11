`timescale 1ns / 1ps

module axis_data_width_adapter #
(
    parameter integer                       AXIS_TDATA_WIDTH_IN     = 16,
    parameter integer                       AXIS_TDATA_WIDTH_OUT    = 32
)
(
    // signals
    input  wire                             aclk,
    
    // axis slave
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH_IN-1:0]   S_AXIS_tdata,
    output wire                             S_AXIS_tready,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH_OUT-1:0]  M_AXIS_tdata
);          
 
    assign M_AXIS_tvalid                    = S_AXIS_tvalid;   
    assign M_AXIS_tdata                     = {{(AXIS_TDATA_WIDTH_IN - AXIS_TDATA_WIDTH_OUT){1'b0}}, S_AXIS_tdata};
    assign S_AXIS_tready                    = M_AXIS_tready;

endmodule