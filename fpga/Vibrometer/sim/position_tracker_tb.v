`timescale 1ns / 1ps

module position_tracker_tb #
(
    parameter integer           AXIS_TDATA_WIDTH    = 32
);

    reg                         SYS_aclk;
    reg                         SYS_aresetn;
    reg [AXIS_TDATA_WIDTH-1:0]  FC_lower_treshold;
    reg [AXIS_TDATA_WIDTH-1:0]  FC_upper_treshold;
    reg                         S_AXIS_tvalid;
    reg [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata;

    integer i                   = 0;

    position_tracker ptracker (
        .SYS_aclk(SYS_aclk),
        .SYS_aresetn(SYS_aresetn),
        .FC_lower_treshold(FC_lower_treshold),
        .FC_upper_treshold(FC_upper_treshold),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .S_AXIS_tdata(S_AXIS_tdata)
    );
     
    initial begin
        SYS_aclk = 0;
        SYS_aresetn = 0;
        FC_lower_treshold = -10;
        FC_upper_treshold = 10;
        S_AXIS_tvalid = 1'b0;

        S_AXIS_tdata = 0;
        SYS_aresetn = 0;
        #8;
                    
        S_AXIS_tdata = -5;
        SYS_aresetn = 1;
        #8;

        for (i=0; i<6; i=i+1) begin
            #8; S_AXIS_tdata = 10; 
            #8; S_AXIS_tdata = 5;
            #8; S_AXIS_tdata = 0;
            #8; S_AXIS_tdata = -5;
            #8; S_AXIS_tdata = -10;
            #8; S_AXIS_tdata = -15;
            #8; S_AXIS_tdata = -10;
            #8; S_AXIS_tdata = -5;
            #8; S_AXIS_tdata = 0;
            #8; S_AXIS_tdata = 5;
            #8; S_AXIS_tdata = 10;
            #8; S_AXIS_tdata = 15;
        end

    end

    always 
        #4 SYS_aclk = !SYS_aclk;
        
    initial begin
        #400
        $finish;
    end

endmodule
