`timescale 1ns / 1ps

module signal_generator_tb();

    logic         aclk    = 0;
    logic [31:0]  GPIO_sg = 3000000;
 
    Signal_Generator_imp_H83IIY DUT (
        .GPIO_sg(GPIO_sg),
        .aclk(SYS_aclk)
    );
     
    always 
        #4 aclk = !aclk;
        
    initial begin
        #1000
        $finish;
    end

endmodule