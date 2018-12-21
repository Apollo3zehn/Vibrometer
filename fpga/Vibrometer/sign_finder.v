`timescale 1ns / 1ps

module sign_finder #
(
    parameter integer                   AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                         SYS_aclk,
    input  wire                         SYS_aresetn,
    
    // SF signals
    input  wire[4:0]                    SF_log_count,
    output wire                         SF_sign,
    
    // axis slave
    input  wire                         S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata,
    output wire                         S_AXIS_tready,
    
    // axis master
    output wire                         M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]  M_AXIS_tdata
);

    localparam                          idle            = 2'b00, 
                                        positive        = 2'b01,
                                        negative        = 2'b10;

    reg                                 previous,       previous_next;
    reg                                 sign,           sign_next;
    reg [31:0]                          count,          count_next;
    reg [1:0]                           state,          state_next;

    wire [31:0]                         max_count;

    assign S_AXIS_tready                = 1'b1;
    assign M_AXIS_tvalid                = 1'b1;
    
    assign SF_sign                      = sign;
    assign max_count                    = 1 << SF_log_count;

    always @(posedge SYS_aclk) begin
            if (~SYS_aresetn) begin
                previous        <= 0;
                sign            <= 1'b1;
                count           <= 0;
                state           <= idle;
            end
            else begin
                previous        <= previous_next;
                sign            <= sign_next;
                count           <= count_next;
                state           <= state_next;
            end
        end
      
        always @* begin
            previous_next       <= previous;
            sign_next           <= sign;
            count_next          <= count;
            state_next          <= state;
        
            case(state)
            
                idle: begin;
                    state_next  <= negative;
                end
                
                negative: begin
                    if (count == max_count - 1) begin
                        previous_next   <= S_AXIS_tdata;
                        sign_next       <= 1'b1;
                        count_next      <= 0;
                        state_next      <= positive;
                    end
                    else if ($signed(S_AXIS_tdata) > $signed(previous)) begin
                        count_next      <= count + 1;
                    end
                end                
                
                positive: begin
                    if (count == max_count - 1) begin
                        previous_next   <= S_AXIS_tdata;
                        sign_next       <= 1'b0;
                        count_next      <= 0;
                        state_next      <= negative;
                    end
                    else if ($signed(S_AXIS_tdata) < $signed(previous)) begin
                        count_next      <= count + 1;
                    end
                end
                
            endcase       
        end

endmodule