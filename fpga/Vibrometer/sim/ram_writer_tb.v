`timescale 1ns / 1ps

module ram_writer_tb();

    reg                         aclk;
    reg                         aresetn;
    reg [31:0]                  GPIO;
    reg                         S_AXIS_tvalid;
    reg [31:0]                  S_AXIS_tdata;
 
    RAM_Writer_imp_DKQNRD rwriter (
        .aclk(aclk),
        .aresetn(aresetn),
        .GPIO(GPIO),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        aclk            = 0;
        aresetn         = 0;
        GPIO            = 0;
        S_AXIS_tvalid   = 1;
        S_AXIS_tdata    = 0;

        #48;
        aresetn         = 1;

                          // 11-7 log_throttle, 6-2 log_length, 1 request, 0 enable
        GPIO            = 31'h00000000_00000000_00000000_00001101;
        #512;

    end

    always begin
        #4;
        aclk            = !aclk;
        S_AXIS_tdata    = S_AXIS_tdata + 1;
    end
        
    initial begin
        #600
        $finish;
    end

endmodule