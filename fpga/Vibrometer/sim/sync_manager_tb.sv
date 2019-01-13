`timescale 1ns / 1ps

module sync_manager_tb #
(
    parameter integer               MM_ADDR_WIDTH   = 32
);

    // system signals
    reg                            aclk;
    reg                            aresetn;
    
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
        aclk = 0;
        aresetn = 0;
        SM_request = 1'b0;
        SM_log_length = 3;
        SM_address = 10;

        aresetn = 0;
        #16;
                    
        aresetn = 1;
        #8;

        // write 4 cycles
        #32;
                
        // read N cycles (and write in parallel)
        SM_request = 1'b1;   
        #48;
        SM_request = 1'b0;
        
        // continue
        #100
        
        // read N cycles (and write in parallel)
        SM_request = 1'b1;   
        #48;
        SM_request = 1'b0;
        #10;

    end

    always 
        #4 aclk = !SYS_aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
