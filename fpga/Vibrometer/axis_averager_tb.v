`timescale 1ns / 1ps

module axis_averager_tb #
(
    parameter integer                   AXIS_TDATA_WIDTH    = 32,
    parameter integer                   BRAM_DATA_WIDTH     = 32,
    parameter integer                   BRAM_ADDR_WIDTH     = 16,
    parameter integer                   RESULT_WIDTH        = 32
);

    reg                         SYS_aclk;
    reg                         SYS_aresetn;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata;
    reg                         S_AXIS_tvalid;
    reg                         AVG_trigger;
    reg                         AVG_user_reset;
    reg [15:0]                  AVG_samples_count;
    reg [RESULT_WIDTH-1:0]      AVG_result_count;
    reg [BRAM_ADDR_WIDTH-1:0]   BRAM_PORTA_rddata;
    reg [BRAM_ADDR_WIDTH-1:0]   BRAM_PORTB_rddata;   
 
    axis_averager averager (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .S_AXIS_tdata(S_AXIS_tdata),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .AVG_trigger(AVG_trigger),
        .AVG_user_reset(AVG_user_reset),
        .AVG_samples_count(AVG_samples_count),
        .AVG_result_count(AVG_result_count),
        .BRAM_PORTA_rddata(BRAM_PORTA_rddata),
        .BRAM_PORTB_rddata(BRAM_PORTB_rddata)
    );
     
    initial begin
        SYS_aresetn = 0;
        S_AXIS_tdata = 0;
        S_AXIS_tvalid = 1;
        AVG_trigger = 0;
        AVG_user_reset = 1;
        AVG_samples_count = 2;
        AVG_result_count = 3;
        BRAM_PORTA_rddata = 1;
        BRAM_PORTB_rddata = 2;
        
        SYS_aclk = 0;
        
        SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; #1;
        SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; #1;
                    
        SYS_aresetn = 1;
        AVG_user_reset = 0;
        SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; #1;
        SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; #1;
        SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; #1;
        SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; #1;
        
        AVG_trigger = 1;
        
        forever #1 SYS_aclk = ~SYS_aclk;
    end

endmodule
