`timescale 1ns / 1ps

module extremum_finder_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         SYS_aclk;
    reg                         SYS_aresetn;
    reg [4:0]                   EF_log_count;
    reg [2:0]                   EF_shift;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata;
    reg                         S_AXIS_tvalid;
 
    extremum_finder efinder (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .EF_log_count(EF_log_count),
        .EF_shift(EF_shift),
        .S_AXIS_tdata(S_AXIS_tdata),
        .S_AXIS_tvalid(S_AXIS_tvalid)
    );
     
    initial begin
        SYS_aclk = 0;
        SYS_aresetn = 0;
        EF_log_count = 0;
        EF_shift = 0;
        S_AXIS_tvalid = 1;

        // 1        
        S_AXIS_tdata = 0;
        SYS_aresetn = 0;
        #8;
                    
        S_AXIS_tdata = -20;
        SYS_aresetn = 1;
        #8;
        
        S_AXIS_tdata = -10;
        #8;
        
        S_AXIS_tdata = 10;
        #8;

        S_AXIS_tdata = 20;
        #8;
        
        S_AXIS_tdata = 10;
        #8;
        
        // start
        EF_log_count = 3;
        S_AXIS_tdata = -10;
        #8;
        
        S_AXIS_tdata = -30;
        #8;
        
        S_AXIS_tdata = -40;
        #8;
        
        S_AXIS_tdata = -20;
        #8;
        
        S_AXIS_tdata = 10;
        #8;
        
        S_AXIS_tdata = 20;       
        #8;
        
        S_AXIS_tdata = 30;
        #8;
                
        S_AXIS_tdata = 40;
        #8;
        
        S_AXIS_tdata = 50;
        #8;
        
        S_AXIS_tdata = 60;
        #8;

        S_AXIS_tdata = 50;
        #8;

        S_AXIS_tdata = 40;
        #8;
        
        forever #1 SYS_aclk = ~SYS_aclk;
    end

    always 
        #4 SYS_aclk = !SYS_aclk;
        
    initial
        #200
        $finish;

endmodule
