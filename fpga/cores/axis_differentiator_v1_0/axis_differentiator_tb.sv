`timescale 1ns / 1ps

module axis_differentiator_tb #
(
     parameter integer                  AXIS_TDATA_WIDTH    = 32
);

    string                              file_path;
    integer                             data_handle; 
    `define NULL                        0    

    logic                               aclk                = 0;
    logic                               aresetn             = 1'b0;
    logic                               enable              = 1'b1;
    logic                               M_AXIS_tready       = 1'b1;
    logic                               S_AXIS_tvalid       = 1'b1;
    logic  [AXIS_TDATA_WIDTH-1:0]       value               = 0;
 
    wire   [AXIS_TDATA_WIDTH-1:0]       S_AXIS_tdata;

    axis_differentiator DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .enable(enable),
        .M_AXIS_tready(M_AXIS_tready),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );

    assign S_AXIS_tdata = value;
     
    initial begin

        $sformat(file_path, "%s/../axis_differentiator.dat", `__FILE__);
        data_handle = $fopen(file_path, "r");

        if (data_handle == `NULL) begin
            $display("file handle is NULL");
            $finish;
        end

        repeat (3) @(posedge aclk); 
            aresetn         = 1'b1;
    end

    always_ff @(posedge aclk) begin
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
