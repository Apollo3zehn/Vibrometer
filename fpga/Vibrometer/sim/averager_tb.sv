`timescale 1ns / 1ps

module averager_tb #
(
    parameter integer               AXIS_TDATA_WIDTH    = 32
);
    
    reg                             aclk                = 0;
    reg                             aresetn             = 0;
    reg  [4:0]                      AV_log_count        = 2;
    reg                             S_AXIS_tvalid       = 1;

    reg                             M_AXIS_tready;
    reg  [AXIS_TDATA_WIDTH-1:0]     S_AXIS_tdata;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] a;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] a_next;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] b;
    reg  [(AXIS_TDATA_WIDTH/2)-1:0] b_next;
 
 
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

    always @(posedge aclk) begin
        if (~aresetn) begin
            a               <= 0;
            b               <= 1;
            S_AXIS_tdata    <= 0;
            M_AXIS_tready   <= 0;
        end else begin
            a               <= a_next;
            b               <= b_next;
            S_AXIS_tdata    <= {b_next, a_next};
            M_AXIS_tready   <= a[2];
        end
    end

    always @* begin
        a_next  = a + 1;
        b_next  = b + 1;
    end

    always 
        #4 aclk = ~aclk;
        
    initial begin
        #2000
        $finish;
    end

endmodule