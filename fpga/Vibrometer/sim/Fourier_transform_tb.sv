`timescale 1ns / 1ps

module Fourier_transform_tb #
(
    parameter integer                   AXIS_TDATA_WIDTH_IN        = 16,
    parameter integer                   AXIS_TDATA_WIDTH_OUT       = 32
);

    string                              file_path_in;
    string                              file_path_out;

    integer                             file_handle_in;
    integer                             file_handle_out;
    
    `define NULL                        0    

    reg                                 aclk                    = 0;
    reg                                 aresetn                 = 0;
    reg  [31:0]                         GPIO                    = 0;
    reg  [AXIS_TDATA_WIDTH_IN-1:0]      value                   = 0;
    reg                                 S_AXIS_filter_tvalid    = 1;
    reg                                 M_AXIS_fft_tready       = 1;
    reg  [(AXIS_TDATA_WIDTH_OUT)-1:0]   M_AXIS_fft_tdata        = 0;
 
    wire [AXIS_TDATA_WIDTH_IN-1:0]      S_AXIS_filter_tdata;
 
    assign S_AXIS_filter_tdata          = value;

    Fourier_Transform_imp_188Q41O DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .GPIO(GPIO),
        .S_AXIS_filter_tdata(S_AXIS_filter_tdata),
        .S_AXIS_filter_tvalid(S_AXIS_filter_tvalid),
        .M_AXIS_fft_tready(M_AXIS_fft_tready),
        .M_AXIS_fft_tdata(M_AXIS_fft_tdata)
    );
     
    initial begin

        $sformat(file_path_in, "%s/../Fourier_Transform_in.dat", `__FILE__);
        $sformat(file_path_out, "%s/../Fourier_Transform_out.dat", `__FILE__);

        file_handle_in  = $fopen(file_path_in, "r");
        file_handle_out = $fopen(file_path_out, "w");

        if (file_handle_in == `NULL) begin
            $display("file handle is NULL");
            $finish;
        end

        if (file_handle_out == `NULL) begin
            $display("file handle is NULL");
            $finish;
        end

        repeat (3) @(posedge aclk); 
            aresetn         = 1'b1;

        repeat (6) @(posedge aclk);
            // 10-6 log_throttle, 5-1 log_count_averages, 0 enable
            GPIO            <= 32'b00000000_00000000_00000000_10000101;
    end

    always @(posedge aclk) begin
        $fscanf(file_handle_in, "%d\n", value);
        $fwrite(
            file_handle_out, 
            "%d;%d;%d;%d\n", 
            $signed(DUT.axis_complex_averager.S_AXIS_tdata[(AXIS_TDATA_WIDTH_OUT/2)-1:0]), 
            $signed(DUT.axis_complex_averager.S_AXIS_tdata[AXIS_TDATA_WIDTH_OUT-1:AXIS_TDATA_WIDTH_OUT/2]), 
            $signed(M_AXIS_fft_tdata[(AXIS_TDATA_WIDTH_OUT/2)-1:0]), 
            $signed(M_AXIS_fft_tdata[AXIS_TDATA_WIDTH_OUT-1:AXIS_TDATA_WIDTH_OUT/2])
        );

        if ($feof(file_handle_in)) begin
            $fclose(file_handle_in);
            $fclose(file_handle_out);
            $finish;
        end
    end

    always 
        #4 aclk = ~aclk;

endmodule
