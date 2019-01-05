`timescale 1ns / 1ps

module signal_generator_tb();

    reg                         SYS_aclk;
    reg [31:0]                  GPIO_sg;
 
    Signal_Generator_imp_H83IIY sgenerator (
        .GPIO_sg(GPIO_sg),
        .aclk(SYS_aclk)
    );
     
    initial begin
    
        SYS_aclk = 0;
        GPIO_sg = 3000000;

        forever #8 SYS_aclk = ~SYS_aclk;
    end

endmodule