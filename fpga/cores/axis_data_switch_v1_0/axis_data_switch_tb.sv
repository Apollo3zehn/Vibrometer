`timescale 1ns / 1ps

module axis_data_switch_tb #
(
     parameter integer        DATA_WIDTH  = 16
);

    logic                     aclk        = 0;
    logic                     switch      = 0;
    logic  [DATA_WIDTH-1:0]   a           = 14;
    logic  [DATA_WIDTH-1:0]   b           = -29;
    
    signal_switch DUT (
        .aclk(aclk),
        .switch(switch),
        .a(a),
        .b(b)
    );
     
    initial begin   
        repeat (3) @(posedge aclk);
            switch  = 1;
        
        repeat (3) @(posedge aclk);
            a       = 7;
            b       = 16;
    end

    always 
        #4 aclk = ~aclk;
        
    initial begin
        #200
        $finish;
    end

endmodule
