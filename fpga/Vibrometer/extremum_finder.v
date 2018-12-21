`timescale 1ns / 1ps

module extremum_finder #
(
    parameter integer                   AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                         SYS_aclk,
    input  wire                         SYS_aresetn,
    
    // EF signals
    input  wire[4:0]                    EF_log_count,
    input  wire[2:0]                    EF_log_shift,
    output wire[AXIS_TDATA_WIDTH-1:0]   EF_lower_treshold,
    output wire[AXIS_TDATA_WIDTH-1:0]   EF_upper_treshold,
    
    // axis slave
    input  wire                         S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata,
    output wire                         S_AXIS_tready,
    
    // axis master
    output wire                         M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]  M_AXIS_tdata
);

    localparam                          idle            = 2'b00, 
                                        measure         = 2'b01;

    reg [AXIS_TDATA_WIDTH-1:0]          min,            min_next;
    reg [AXIS_TDATA_WIDTH-1:0]          max,            max_next;
    reg [31:0]                          count,          count_next;
    reg [1:0]                           state,          state_next;
    
    reg [AXIS_TDATA_WIDTH-1:0]          tmp_min;
    reg [AXIS_TDATA_WIDTH-1:0]          tmp_max;
    
    wire [31:0]                         max_count;

    assign S_AXIS_tready                = 1'b1;
    assign M_AXIS_tvalid                = S_AXIS_tvalid;
    assign M_AXIS_tdata                 = S_AXIS_tdata;
    assign EF_lower_treshold            = min;
    assign EF_upper_treshold            = max;

    assign max_count                    = 1 << EF_log_count;

    always @(posedge SYS_aclk) begin
            if (~SYS_aresetn) begin
                min             <= {1'b0, {(AXIS_TDATA_WIDTH-1){1'b1}}}; // max. positve number
                max             <= {1'b1, {(AXIS_TDATA_WIDTH-1){1'b0}}}; // max. negative number
                count           <= 0;
                state           <= idle;
            end
            else begin
                min             <= min_next;
                max             <= max_next;
                count           <= count_next;
                state           <= state_next; 
            end
        end
      
        always @* begin
            min_next            <= min;
            max_next            <= max;
            count_next          <= count;
            state_next          <= state;
        
            case(state)
            
                idle: begin
                    tmp_min         <= {1'b0, {(AXIS_TDATA_WIDTH-1){1'b1}}}; // max. positve number
                    tmp_max         <= {1'b1, {(AXIS_TDATA_WIDTH-1){1'b0}}}; // max. negative number
                    count_next      <= 0;
                    state_next      <= measure;
                end
                    
                measure: begin
                    if ($signed(S_AXIS_tdata) < $signed(tmp_min))
                        tmp_min     <= S_AXIS_tdata;

                    if ($signed(S_AXIS_tdata) > $signed(tmp_max))
                        tmp_max     <= S_AXIS_tdata;
                        
                    if (count == max_count - 1) begin
                        min_next    <= $signed(tmp_min) >>> EF_log_shift;
                        max_next    <= $signed(tmp_max) >>> EF_log_shift;
                        state_next  <= idle;
                    end
                        
                    count_next      <= count + 1;
                end
                                 
            endcase       
        end

endmodule
