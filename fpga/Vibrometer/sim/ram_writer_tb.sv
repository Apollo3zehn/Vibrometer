`timescale 1ns / 1ps

module ram_writer_tb();

    reg                         aclk;
    reg                         aresetn;
    reg [31:0]                  GPIO;
    reg                         S_AXIS_tvalid;
    reg [31:0]                  S_AXIS_tdata;
    reg                         M_AXI_position_awready;
    reg                         M_AXI_position_wready;
 
    RAM_Writer_imp_DKQNRD rwriter (
        .aclk(aclk),
        .aresetn(aresetn),
        .GPIO(GPIO),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata),
        .M_AXI_position_awready(M_AXI_position_awready),
        .M_AXI_position_wready(M_AXI_position_wready)
    );
     
    initial begin
        aclk                        = 1;
        aresetn                     = 0;
        GPIO                        = 0;
        S_AXIS_tvalid               = 1;
        S_AXIS_tdata                = 0;
        M_AXI_position_awready      = 1;
        M_AXI_position_wready       = 1;
        
        #40;
        aresetn                     = 1;
        #48;
        
        // 11-7 log_throttle, 6-2 log_length, 1 request, 0 enable
        GPIO                        = 32'b00000000_00000000_00000000_10010001;
        #400;

        GPIO                        = 32'b00000000_00000000_00000000_10010011;
        #400;

        GPIO                        = 32'b00000000_00000000_00000000_10010001;
        #400;

    end

    always begin
        #8; 
        S_AXIS_tdata                = S_AXIS_tdata + 1;
    end

    always begin
        #4;
        aclk                        = !aclk;
    end

    initial begin
        #2000
        $finish;
    end

endmodule