`timescale 1ns / 1ps

module differentiator_tb #
(
     parameter integer                  AXIS_TDATA_WIDTH    = 32
);

    integer                             data_handle; 
    `define NULL                        0    

    reg                                 aclk                = 0;
    reg                                 aresetn             = 0'b0;
    reg                                 M_AXIS_tready       = 1'b1;
    reg                                 S_AXIS_tvalid       = 1'b1;
    reg     [AXIS_TDATA_WIDTH - 1:0]    value               = 0;

    wire [AXIS_TDATA_WIDTH-1:0]         S_AXIS_tdata;

    differentiator DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .M_AXIS_tready(M_AXIS_tready),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );

    assign S_AXIS_tdata = value;
     
    initial begin

        data_handle = $fopen("C:/Users/wilvin/Desktop/Git/Vibrometer/fpga/Vibrometer/sim/differentiator.dat", "r");

        if (data_handle == `NULL) begin
            $display("file handle is NULL");
            $finish;
        end

        repeat (3) @(posedge aclk); 
            aresetn         = 1'b1;
    end

    always @(posedge aclk) begin
        $fscanf(data_handle, "%d\n", value);

        if ($feof(data_handle)) begin
            $finish;
        end
    end

    always 
        #8 S_AXIS_tvalid = ~S_AXIS_tvalid;

    always 
        #4 aclk = ~aclk;

endmodule
