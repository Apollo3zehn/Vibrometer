`timescale 1ns / 1ps

module position_tracker #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,
    
    // FC signals
    input  wire [(AXIS_TDATA_WIDTH/2)-1:0]  FC_lower_threshold,
    input  wire [(AXIS_TDATA_WIDTH/2)-1:0]  FC_upper_threshold,
    
    // axis slave
    input  wire                             S_AXIS_tvalid,
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    output wire                             S_AXIS_tready,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata
);

    localparam                              idle            = 2'b00, 
                                            low             = 2'b01, 
                                            high            = 2'b10;              

    reg  [AXIS_TDATA_WIDTH-1:0]             position,       position_next;
    reg  [1:0]                              state,          state_next;                
    reg  [AXIS_TDATA_WIDTH/2-1:0]           center;
    
    wire [AXIS_TDATA_WIDTH/2-1:0]           signal_a;
    wire [AXIS_TDATA_WIDTH/2-1:0]           signal_b;
  
    assign S_AXIS_tready                    = 1'b1;
    assign M_AXIS_tvalid                    = 1'b1;
    assign M_AXIS_tdata                     = position;

    assign signal_a                         = S_AXIS_tdata[(AXIS_TDATA_WIDTH/2)-1:0];
    assign signal_b                         = S_AXIS_tdata[AXIS_TDATA_WIDTH-1:AXIS_TDATA_WIDTH/2];

    always @(posedge aclk) begin
        if (~aresetn) begin
            position        <= 0;
            state           <= idle;
        end
        else begin
            position        <= position_next;
            state           <= state_next;
        end
    end
      
    always @* begin
        position_next       = position;
        state_next          = state;
    
        case(state)
        
            idle: begin
                if ($signed(signal_a) < $signed(FC_lower_threshold))
                    state_next  = low;
            end
                
            low: begin
                if ($signed(signal_a) > $signed(FC_upper_threshold))
                    state_next  = high;
            end
            
            high: begin
                if ($signed(signal_a) < $signed(FC_lower_threshold)) begin
                
                    center = (($signed(FC_upper_threshold) + $signed(FC_lower_threshold)) >>> 1);
                
                    if ($signed(signal_b) > $signed(center))
                        position_next = $signed(position) + 1;
                    else
                        position_next = $signed(position) - 1;
                        
                    state_next  = low;             
                end
            end
            
        endcase       
    end

endmodule