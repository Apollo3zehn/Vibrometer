`timescale 1ns / 1ps

module signal_generator_tb();

    reg                         aclk;
    reg [31:0]                  GPIO_sg;
 
    Signal_Generator_imp_H83IIY sgenerator (
        .GPIO_sg(GPIO_sg),
        .aclk(SYS_aclk)
    );
     
    initial begin
    
        aclk = 0;
        GPIO_sg = 3000000;

        forever #8 aclk = ~SYS_aclk;
    end

endmodule