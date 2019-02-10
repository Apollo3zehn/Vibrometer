`timescale 1ns / 1ps

module axis_throttler #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,
    
    // IP signals
    input  wire [4:0]                       log_throttle,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata,

    // axis slave
    output wire                             S_AXIS_tready,
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata
);         

    reg         [31:0]                      count,              count_next;
    reg                                     tvalid,             tvalid_next;

    wire        [31:0]                      max;
    
    assign      S_AXIS_tready               = M_AXIS_tready;
    assign      M_AXIS_tvalid               = tvalid;
    assign      M_AXIS_tdata                = S_AXIS_tdata;

    assign      max                         = 1 << log_throttle;

    always @(posedge aclk) begin
        if (~aresetn) begin
            count           <= 0;
            tvalid          <= 0;
        end else begin
            count           <= count_next;
            tvalid          <= tvalid_next;
        end
    end
      
    always @* begin
        count_next          = count + 1;
        tvalid_next         = 0;

        if (count >= max - 1) begin
            count_next      = 0;
            tvalid_next     = S_AXIS_tvalid;
        end   
    end

endmodule