// https://www.dsprelated.com/showarticle/814.php
`timescale 1ns / 1ps

module axis_differentiator #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,

    // IP signals
    input  wire                             enable,

    // axis slave
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    output wire                             S_AXIS_tready,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata
);
    genvar                                  i;
    
    logic  signed [AXIS_TDATA_WIDTH-1:0]    shift1,                 shift1_next;
    logic  signed [AXIS_TDATA_WIDTH-1:0]    shift2,                 shift2_next;
    logic  signed [AXIS_TDATA_WIDTH-1:0]    shift3,                 shift3_next;
    logic  signed [AXIS_TDATA_WIDTH-1:0]    result,                 result_next;
    logic  signed [AXIS_TDATA_WIDTH-1:0]    shift_register[4:0],    shift_register_next[4:0];

    wire   signed [AXIS_TDATA_WIDTH:0]      sum1;                   // size = AXIS_TDATA_WIDTH + 1
    wire   signed [AXIS_TDATA_WIDTH:0]      sum2;                   // size = AXIS_TDATA_WIDTH + 1

    assign sum1             = shift_register[4] - shift_register[0];
    assign sum2             = shift_register[1] - shift_register[3];    
    assign M_AXIS_tdata     = enable ? result : S_AXIS_tdata;
    assign M_AXIS_tvalid    = S_AXIS_tvalid;

    assign S_AXIS_tready    = aresetn;

    for (i = 0; i < 5 ; i = i + 1) begin
        always_ff @(posedge aclk) begin
            if (~aresetn)
                shift_register[i] <= 0;
            else
                shift_register[i] <= shift_register_next[i];
        end       
    end

    always_ff @(posedge aclk) begin
        if (~aresetn) begin
            shift1    <= 0;
            shift2    <= 0;
            shift3    <= 0;
            result    <= 0;
        end else begin
            shift1 <= shift1_next;
            shift2 <= shift2_next;
            shift3 <= shift3_next;
            result <= result_next;
        end
    end     

    for (i = 1; i < 5 ; i = i + 1) begin
        always_comb begin  
            if (S_AXIS_tvalid)
                shift_register_next[i] = shift_register[i - 1];
            else
                shift_register_next[i] = shift_register[i];
        end
    end

    always_comb begin 
        shift1_next                 = shift1;
        shift2_next                 = shift2;
        shift3_next                 = shift3;
        result_next                 = result;
        shift_register_next[0]      = shift_register[0];

        if (S_AXIS_tvalid) begin
            shift_register_next[0]  = S_AXIS_tdata;
            shift1_next             = sum1 >>> 3;
            shift2_next             = sum1 >>> 4;
            shift3_next             = sum2 >>> 5;
            result_next             = shift1 + shift2 + sum2 - shift3;
        end
    end

endmodule