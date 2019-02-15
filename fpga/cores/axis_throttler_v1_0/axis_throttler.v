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
    reg                                     deny,               deny_next;

    wire        [31:0]                      max;
    
    assign      S_AXIS_tready               = M_AXIS_tready && ~deny;
    assign      M_AXIS_tvalid               = S_AXIS_tvalid && ~deny;
    assign      M_AXIS_tdata                = S_AXIS_tdata;

    assign      max                         = 1 << log_throttle;

    always @(posedge aclk) begin
        if (~aresetn) begin
            count           <= 0;
            deny            <= 1'b0;
        end else begin
            count           <= count_next;
            deny            <= deny_next;
        end
    end
      
    always @* begin
        deny_next           = deny;
        count_next          = count;

        if (S_AXIS_tvalid) begin
            if (count >= max - 1) begin
                count_next      = 0;
                deny_next       = 1'b0;
            end else begin
                count_next      = count + 1;
                deny_next       = 1'b1;
            end
        end
    end

endmodule