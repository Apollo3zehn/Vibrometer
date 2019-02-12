`timescale 1ns / 1ps

module axis_counter #
(
    parameter integer                           AXIS_TDATA_WIDTH  = 32,
    parameter integer                           COUNTER_WIDTH     = 32
)
(
    // system signals
    input  wire                                 aclk,
    input  wire                                 aresetn,

    // axis master
    input  wire                                 M_AXIS_tready,
    output wire                                 M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]          M_AXIS_tdata
);

    reg  [COUNTER_WIDTH-1:0]                    data, data_next;

    assign M_AXIS_tvalid                        = aresetn;
    assign M_AXIS_tdata                         = data;
    assign counter                              = data;

    always @(posedge aclk) begin
        if (~aresetn) begin
            data            <= 0;
        end else begin
            data            <= data_next;
        end
    end
      
    always @* begin
        data_next           = data;

        if (M_AXIS_tready) begin
            data_next       = data + 1;
        end
    end

endmodule