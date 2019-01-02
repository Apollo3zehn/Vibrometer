`timescale 1ns / 1ps

module sync_manager_tb #
(
    parameter integer               MM_ADDR_WIDTH   = 32
);

    // system signals
    reg                            SYS_aclk;
    reg                            SYS_aresetn;
    
    // SM signals
    reg                            SM_request;
    reg [4:0]                      SM_log_length;
    reg [MM_ADDR_WIDTH-1:0]        SM_address;
 
    sync_manager smanager (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .SM_request(SM_request),
        .SM_log_length(SM_log_length),
        .SM_address(SM_address)
    );
     
    initial begin
        SYS_aclk = 0;
        SYS_aresetn = 0;
        SM_request = 1'b0;
        SM_log_length = 3;
        SM_address = 10;

        SYS_aresetn = 0;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
                    
        SYS_aresetn = 1;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;

        // write 4 cycles
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
                
        // read N cycles (and write in parallel)
        SM_request = 1'b1;   
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        SM_request = 1'b0;
        
        // continue
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; 
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk; 
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        // read N cycles (and write in parallel)
        SM_request = 1'b1;   
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;       
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        SM_request = 1'b0;
                
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        #1; SYS_aclk = ~SYS_aclk; #1; SYS_aclk = ~SYS_aclk;
        
        forever #1 SYS_aclk = ~SYS_aclk;
    end

endmodule
