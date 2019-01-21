// https://www.dsprelated.com/showarticle/814.php
`timescale 1ns / 1ps

module differentiator #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,

    // axis slave
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    output wire                             S_AXIS_tready,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata
);
    genvar                                  i, j;
    reg  signed [AXIS_TDATA_WIDTH-1:0]      shift_register[4:0], shift_register_next[4:0];

    wire signed [AXIS_TDATA_WIDTH-1:0]      sum1;
    wire signed [AXIS_TDATA_WIDTH-1:0]      sum2;

    assign sum1             = shift_register[4] - shift_register[0];
    assign sum2             = shift_register[1] - shift_register[3];
    
    assign M_AXIS_tdata     = (sum1 >>> 3) + (sum1 >>> 4) + sum2 - (sum2 >>> 5);
    assign M_AXIS_tvalid    = S_AXIS_tvalid;

    assign S_AXIS_tready    = 1'b1;

    for (i = 0; i < 5 ; i = i + 1) begin
        always @(posedge aclk) begin
            if (~aresetn)
                shift_register[i] <= 0;
            else
                shift_register[i] <= shift_register_next[i];
        end       
    end

    for (i = 1; i < 5 ; i = i + 1) begin
        always @* begin 
            if (S_AXIS_tvalid)
                shift_register_next[i] = shift_register[i - 1];
            else
                shift_register_next[i] = shift_register[i];
        end
    end

    always @* begin
        if (S_AXIS_tvalid)
            shift_register_next[0] = S_AXIS_tdata;
        else
            shift_register_next[0] = shift_register[0];
    end

endmodule