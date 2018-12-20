`timescale 1 ns / 1 ps

module axis_averager #
(
    parameter integer                   AXIS_TDATA_WIDTH    = 32,
    parameter integer                   BRAM_DATA_WIDTH     = 32,
    parameter integer                   BRAM_ADDR_WIDTH     = 16,
    parameter integer                   RESULT_WIDTH        = 32
)
(
    // system signals
    input  wire                         SYS_aclk,
    input  wire                         SYS_aresetn,
    
    // axis slave
    output wire                         S_AXIS_tready,
    input  wire [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata,
    input  wire                         S_AXIS_tvalid,
    
    // averager specific ports
    input  wire                         AVG_trigger,
    input  wire                         AVG_user_reset,
    input  wire [15:0]                  AVG_samples_count,
    input  wire [RESULT_WIDTH-1:0]      AVG_result_count,
    output wire 						AVG_is_finished,
    output wire [RESULT_WIDTH-1:0]      AVG_result,
    
    // BRAM PORT A
    output wire [BRAM_ADDR_WIDTH-1:0]   BRAM_PORTA_addr,
    output wire                         BRAM_PORTA_clk,
    output wire [BRAM_DATA_WIDTH-1:0]   BRAM_PORTA_wrdata,
    input  wire [BRAM_DATA_WIDTH-1:0]   BRAM_PORTA_rddata,
    output wire                         BRAM_PORTA_rst,
    output wire                         BRAM_PORTA_we,
    
    // BRAM PORT B
    output wire [BRAM_ADDR_WIDTH-1:0]   BRAM_PORTB_addr,
    output wire                         BRAM_PORTB_clk,
    output wire [BRAM_DATA_WIDTH-1:0]   BRAM_PORTB_wrdata,
    input  wire [BRAM_DATA_WIDTH-1:0]   BRAM_PORTB_rddata,
    output wire                         BRAM_PORTB_rst,
    output wire                         BRAM_PORTB_we
);

    reg [BRAM_ADDR_WIDTH-1:0]           address_A,          address_A_next;
    reg [BRAM_ADDR_WIDTH-1:0]           address_B,          address_B_next;
    reg [2:0]                           state,              state_next;
    reg                                 is_writable,        is_writable_next;
    reg                                 is_finished,        is_finished_next;
    reg [RESULT_WIDTH-1:0]              result,             result_next;
    reg [BRAM_DATA_WIDTH-1:0]           data,               data_next;
    
    localparam                          reset           = 3'b000, 
                                        clear_bram      = 3'b001, 
                                        wait_trigger    = 3'b010,
                                        measure         = 3'b011,
                                        finish          = 3'b100;
  
    assign S_AXIS_tready                = 1'b1;
    assign AVG_is_finished              = is_finished;
    assign AVG_result                   = result;
    
    assign BRAM_PORTA_clk               = SYS_aclk;
    assign BRAM_PORTA_rst               = ~SYS_aresetn;
    assign BRAM_PORTA_addr              = address_A;
    assign BRAM_PORTA_wrdata            = data;
    assign BRAM_PORTA_we                = is_writable;
    
    assign BRAM_PORTB_clk               = SYS_aclk;
    assign BRAM_PORTB_rst               = ~SYS_aresetn;
    assign BRAM_PORTB_addr              = address_B;
    assign BRAM_PORTB_wrdata            = {BRAM_DATA_WIDTH{1'b0}};
    assign BRAM_PORTB_we                = 1'b0;

    always @(posedge SYS_aclk) begin
        if (~SYS_aresetn || AVG_user_reset) begin
            address_A       <= {(BRAM_ADDR_WIDTH){1'b0}};
            address_B       <= {(BRAM_ADDR_WIDTH){1'b0}};
            result          <= {(RESULT_WIDTH){1'b0}};
            data            <= {(BRAM_DATA_WIDTH){1'b0}};
            is_writable     <= 1'b0;
            is_finished     <= 1'b0;
            state           <= reset;
        end
        else begin
            address_A       <= address_A_next;
            address_B       <= address_B_next;
            result          <= result_next;
            data            <= data_next;                        
            is_writable     <= is_writable_next;
            is_finished     <= is_finished_next;
            state           <= state_next;
        end
    end
  
    always @* begin
        address_A_next      <= address_A;
        address_B_next      <= address_B;
        state_next          <= state;
        is_writable_next    <= is_writable;
        result_next         <= result;
        data_next           <= data;
        is_finished_next    <= is_finished;
    
        case(state)
            reset: begin
                    address_A_next      <= {(BRAM_ADDR_WIDTH){1'b0}};
                    address_B_next      <= {(BRAM_ADDR_WIDTH){1'b0}};
                    result_next         <= {(RESULT_WIDTH){1'b0}};
                    state_next          <= clear_bram;
                    is_writable_next    <= 1'b1;
                    is_finished_next    <= 1'b0;
                    data_next           <= {(BRAM_DATA_WIDTH){1'b0}};
                end               
            clear_bram: begin
                    address_A_next <= address_A + 1'b1;
    
                    if (address_A == AVG_samples_count-1) begin
                        state_next          <= wait_trigger;
                        is_writable_next    <= 1'b0;
                    end
                end            
            wait_trigger: begin
                    address_A_next      <= -2;
                    address_B_next      <= 0;
                    is_writable_next    <= 1'b0;
        
                    if (AVG_trigger) begin
                    
                        result_next <= result + 1;
                        
                        if (result == AVG_result_count)
                            state_next <= finish;
                        else
                            state_next <= measure;          
                    end
                end       
            measure: begin
                    if (S_AXIS_tvalid) begin
                        address_A_next      <= address_A + 1;
                        address_B_next      <= address_B + 1;
                        data_next           <= BRAM_PORTB_rddata + S_AXIS_tdata;
                        is_writable_next    <= 1'b1;
                        
                        if (address_A == AVG_samples_count-2)
                            state_next <= wait_trigger;
                    end
                    else begin
                        is_writable_next <= 1'b0;
                    end
                end
            finish: begin
                    is_finished_next <= 1'b1;
                end               
        endcase       
    end
endmodule
