`timescale 1ns / 1ps

module averager_tb #
(
    parameter integer               AXIS_TDATA_WIDTH    = 32
);
    
    reg                             aclk                = 0;
    reg                             aresetn             = 0;
    reg  [4:0]                      AV_log_count        = 2;
    reg                             S_AXIS_tvalid       = 1;
    reg                             M_AXIS_tready       = 1;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] a                   = 0;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] a_next              = 0;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] b                   = 1;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] b_next              = 1;
 
    wire [AXIS_TDATA_WIDTH-1:0]     S_AXIS_tdata        = 0;
 
    averager DUT (
        .aclk(aclk),
        .aresetn(aresetn),
        .AV_log_count(AV_log_count),
        .S_AXIS_tdata(S_AXIS_tdata),
        .S_AXIS_tvalid(S_AXIS_tvalid),
        .M_AXIS_tready(M_AXIS_tready)
    );
     
    initial begin
        repeat (3) @(posedge aclk); 
            aresetn         = 1;        
    end
    
    initial begin
        repeat (3) @(posedge aclk); 
            aresetn         = 1;       
    end

    assign S_AXIS_tdata = {b, a};

    always @(posedge aclk) begin
        a <= a + 1;
        b <= b + 1;
    end
        
    always 
        #4 aclk = ~aclk;
        
    initial begin
        #2000
        $finish;
    end

endmodule