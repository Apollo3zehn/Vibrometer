`timescale 1ns / 1ps

module signal_switch_tb #
(
     parameter integer              DATA_WIDTH      = 16
);

    reg                             aclk;
    reg                             switch;
    reg  [DATA_WIDTH-1:0]           a;
    reg  [DATA_WIDTH-1:0]           b;
    
    signal_switch sswitch (
        .SYS_aclk(SYS_aclk),
        .switch(switch),
        .a(a),
        .b(b)
    );
     
    initial begin
        aclk = 0;
        switch = 0;
        a = 14;
        b = -29;
    
        #24;

        switch = 1;
        
        #24;
        
        a = 7;
        b = 16;
    end

    always 
        #4 aclk = !SYS_aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
