`timescale 1ns / 1ps

module axis_extremum_finder #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,
    
    // IP signals
    input  wire [4:0]                       log_count,
    input  wire [2:0]                       shift,
    output wire [AXIS_TDATA_WIDTH/2-1:0]    lower_threshold,
    output wire [AXIS_TDATA_WIDTH/2-1:0]    upper_threshold,
    
    // axis slave
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    output wire                             S_AXIS_tready
);

    localparam                              idle            = 1'b0, 
                                            measure         = 1'b1;

    logic  [AXIS_TDATA_WIDTH/2-1:0]         min,            min_next;
    logic  [AXIS_TDATA_WIDTH/2-1:0]         max,            max_next;
    logic  [AXIS_TDATA_WIDTH/2-1:0]         tmp_min,        tmp_min_next;
    logic  [AXIS_TDATA_WIDTH/2-1:0]         tmp_max,        tmp_max_next;
    logic  [31:0]                           count,          count_next;
    logic                                   state,          state_next;
    
    logic  [AXIS_TDATA_WIDTH/2-1:0]         tmp_center;
    
    logic  [AXIS_TDATA_WIDTH/2-1:0]         testreg;
    
    wire   [AXIS_TDATA_WIDTH/2-1:0]         signal_a;
    wire   [AXIS_TDATA_WIDTH/2-1:0]         signal_b; 
    wire   [31:0]                           max_count;

    assign S_AXIS_tready                    = aresetn;
    assign lower_threshold                  = min;
    assign upper_threshold                  = max;

    assign signal_a                         = S_AXIS_tdata[AXIS_TDATA_WIDTH/2-1:0];
    assign signal_b                         = S_AXIS_tdata[AXIS_TDATA_WIDTH-1:AXIS_TDATA_WIDTH/2];
    assign max_count                        = 1 << log_count;

    always_ff @(posedge aclk) begin
        if (~aresetn) begin
            min             <= {1'b0, {(AXIS_TDATA_WIDTH/2-1){1'b1}}}; // max. positve number
            max             <= {1'b1, {(AXIS_TDATA_WIDTH/2-1){1'b0}}}; // max. negative number
            tmp_min         <= {1'b0, {(AXIS_TDATA_WIDTH/2-1){1'b1}}}; // max. positve number
            tmp_max         <= {1'b1, {(AXIS_TDATA_WIDTH/2-1){1'b0}}}; // max. negative number
            count           <= 0;
            state           <= idle;
        end else begin
            min             <= min_next;
            max             <= max_next;
            tmp_min         <= tmp_min_next;
            tmp_max         <= tmp_max_next;
            count           <= count_next;
            state           <= state_next; 
        end
    end
  
    always_comb begin 
        min_next            = min;
        max_next            = max;
        tmp_min_next        = tmp_min;
        tmp_max_next        = tmp_max;
        count_next          = count;
        state_next          = state;
    
        case(state)
        
            idle: begin
                tmp_min_next        = {1'b0, {(AXIS_TDATA_WIDTH/2-1){1'b1}}}; // max. positve number
                tmp_max_next        = {1'b1, {(AXIS_TDATA_WIDTH/2-1){1'b0}}}; // max. negative number
                count_next          = 0;
                state_next          = measure;
            end
                
            measure: begin
                if ($signed(signal_a) < $signed(tmp_min))
                    tmp_min_next    = $signed(signal_a);
                    
                if ($signed(signal_a) > $signed(tmp_max))
                    tmp_max_next    = $signed(signal_a);
                    
                if (count >= max_count - 1) begin
                    tmp_center      = ($signed(tmp_max) + $signed(tmp_min)) >>> 1;
                    min_next        = (($signed(tmp_min) - $signed(tmp_center)) >>> shift) + $signed(tmp_center);
                    max_next        = (($signed(tmp_max) - $signed(tmp_center)) >>> shift) + $signed(tmp_center);
                    state_next      = idle;
                end
                    
                count_next = count + 1;
            end
                             
        endcase
    end

endmodule
