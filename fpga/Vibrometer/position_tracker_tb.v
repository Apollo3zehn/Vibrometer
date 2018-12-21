`timescale 1ns / 1ps

module position_tracker_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         SYS_aclk;
    reg                         SYS_aresetn;
    reg                         FC_sign;
    reg [AXIS_TDATA_WIDTH-1:0]  FC_lower_treshold;
    reg [AXIS_TDATA_WIDTH-1:0]  FC_upper_treshold;
    reg                         S_AXIS_tvalid;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata;
 
    position_tracker ptracker (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .FC_sign(FC_sign),
        .FC_lower_treshold(FC_lower_treshold),
        .FC_upper_treshold(FC_upper_treshold),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        SYS_aclk = 0;
        SYS_aresetn = 0;
        FC_sign = 1'b0;
        FC_lower_treshold = -10;
        FC_upper_treshold = 10;
        S_AXIS_tvalid = 1'b0;

        S_AXIS_tdata = 0;
        SYS_aresetn = 0;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
                    
        S_AXIS_tdata = -5;
        SYS_aresetn = 1;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;

        // cylce 1
        FC_sign = 1'b1;        
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // cylce 2
        FC_sign = 1'b1;        
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // cylce 3   
        FC_sign = 1'b1;     
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // cylce 4  
        FC_sign = 1'b1;      
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // cylce 5   
        FC_sign = 1'b0;     
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // cylce 6  
        FC_sign = 1'b0;      
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // cylce 7     
        FC_sign = 1'b0;   
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        S_AXIS_tdata = -10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = -5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 0; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 5; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 10; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        S_AXIS_tdata = 15; #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        forever #1 SYS_aclk = ~SYS_aclk;
    end

endmodule
