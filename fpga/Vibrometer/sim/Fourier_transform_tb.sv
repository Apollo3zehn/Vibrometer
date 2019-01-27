`timescale 1ns / 1ps

module Fourier_transform_tb #
(
    parameter integer                   AXIS_TDATA_WIDTH        = 16
);
    integer                             file_handle_in;
    integer                             file_handle_out;
    
    `define NULL                        0    

    reg                                 aclk                    = 0;
    reg                                 aresetn                 = 0;
    reg  [AXIS_TDATA_WIDTH-1:0]         value                   = 0;
    reg                                 S_AXIS_filter_tvalid    = 1;
    reg                                 M_AXIS_fft_tready       = 1;
    reg  [(AXIS_TDATA_WIDTH*2)-1:0]     M_AXIS_fft_tdata        = 0;
 
    wire [AXIS_TDATA_WIDTH-1:0]         S_AXIS_filter_tdata;
 
    assign S_AXIS_filter_tdata          = value;

    Fourier_Transform_imp_188Q41O DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .S_AXIS_filter_tdata(S_AXIS_filter_tdata),
        .S_AXIS_filter_tvalid(S_AXIS_filter_tvalid),
        .M_AXIS_fft_tready(M_AXIS_fft_tready),
        .M_AXIS_fft_tdata(M_AXIS_fft_tdata)
    );
     
    initial begin

        file_handle_in  = $fopen("C:/Users/wilvin/Desktop/Git/Vibrometer/fpga/Vibrometer/sim/Fourier_Transform_in.dat", "r");
        file_handle_out = $fopen("C:/Users/wilvin/Desktop/Git/Vibrometer/fpga/Vibrometer/sim/Fourier_Transform_out.dat", "w");

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
    end

    always @(posedge aclk) begin
        $fscanf(file_handle_in, "%d\n", value);
        $fwrite(file_handle_out, "%h\n", M_AXIS_fft_tdata);

        if ($feof(file_handle_in)) begin
            $fclose(file_handle_in);
            $fclose(file_handle_out);
            $finish;
        end
    end

    always 
        #4 aclk = ~aclk;

endmodule