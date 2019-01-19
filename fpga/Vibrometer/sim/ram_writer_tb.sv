`timescale 1ns / 1ps

module ram_writer_tb();

    reg         aclk                    = 0;
    reg         aresetn                 = 0;
    reg [31:0]  GPIO                    = 0;
    reg         S_AXIS_tvalid           = 1;
    reg [31:0]  S_AXIS_tdata            = 0;
    reg         M_AXI_position_awready  = 1;
    reg         M_AXI_position_wready   = 1;
 
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
        repeat (5) @(posedge aclk);
            aresetn         <= 1;

        repeat (6) @(posedge aclk);
            // 11-7 log_throttle, 6-2 log_length, 1 request, 0 enable
            GPIO            <= 32'b00000000_00000000_00000000_10011001;

        repeat (100) @(posedge aclk);
            GPIO            <= 32'b00000000_00000000_00000000_10011011;

        repeat (100) @(posedge aclk);
            GPIO            <= 32'b00000000_00000000_00000000_10011001;

        repeat (100) @(posedge aclk);
            GPIO            <= 32'b00000000_00000000_00000000_10011000;

        repeat (100) @(posedge aclk);
            GPIO            <= 32'b00000000_00000000_00000000_10011001;

        repeat (100) @(posedge aclk);
            GPIO            <= 32'b00000000_00000000_00000000_10011011;

        repeat (100) @(posedge aclk);
            GPIO            <= 32'b00000000_00000000_00000000_10011001;
    end

    always begin
        repeat (1) @(posedge aclk); 
            S_AXIS_tdata    <= S_AXIS_tdata + 1;
    end

    always 
        #4 aclk = ~aclk;

    initial begin
        #10000
        $finish;
    end

endmodule