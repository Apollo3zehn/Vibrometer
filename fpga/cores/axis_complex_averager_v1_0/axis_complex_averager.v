`timescale 1ns / 1ps

module axis_complex_averager #
(
    parameter integer                       AXIS_TDATA_WIDTH    = 32,
    parameter integer                       BRAM_DATA_WIDTH     = 64,
    parameter integer                       BRAM_ADDR_WIDTH     = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,

    // IP signals
    input  wire [4:0]                       log_count,
    
    // slave
    input  wire [AXIS_TDATA_WIDTH-1:0]      S_AXIS_tdata,
    input  wire                             S_AXIS_tvalid,
    output wire                             S_AXIS_tready,

    // master
    input  wire                             M_AXIS_tready,
    output wire [AXIS_TDATA_WIDTH-1:0]      M_AXIS_tdata,
    output wire                             M_AXIS_tvalid,
    output wire                             M_AXIS_tlast,

    // BRAM port A
    output wire [BRAM_ADDR_WIDTH-1:0]       bram_porta_addr,
    output wire                             bram_porta_clk,
    output wire [BRAM_DATA_WIDTH-1:0]       bram_porta_wrdata,
    output wire                             bram_porta_we,
    
    // BRAM port B
    output wire [BRAM_ADDR_WIDTH-1:0]       bram_portb_addr,
    output wire                             bram_portb_clk,
    output wire                             bram_portb_en,
    input  wire [BRAM_DATA_WIDTH-1:0]       bram_portb_rddata
);
    function [(AXIS_TDATA_WIDTH/2)-1:0] truncate(input [(BRAM_DATA_WIDTH/2)-1:0] val32);
        truncate = val32[(AXIS_TDATA_WIDTH/2)-1:0];
    endfunction

    genvar                                  i;

    localparam                              SIGN_EXTENSION      = (BRAM_DATA_WIDTH - AXIS_TDATA_WIDTH) / 2;

    localparam                              first               = 1'b0, 
                                            measure             = 1'b1;

    reg  [7:0]                              avg_count,          avg_count_next;
    reg                                     state,              state_next;
    reg  [BRAM_ADDR_WIDTH-1:0]              a_addr,             a_addr_next;
    reg  [BRAM_ADDR_WIDTH-1:0]              b_addr,             b_addr_next;
    reg                                     t_last,             t_last_next;

    wire [(BRAM_DATA_WIDTH/2)-1:0]          s_real;
    wire [(BRAM_DATA_WIDTH/2)-1:0]          s_imag;
    wire [(BRAM_DATA_WIDTH/2)-1:0]          b_real;
    wire [(BRAM_DATA_WIDTH/2)-1:0]          b_imag;
    wire [31:0]                             max_count;
    wire                                    write_enable;

    assign max_count                        = 1 << log_count;
    assign write_enable                     = M_AXIS_tready && S_AXIS_tvalid && aresetn;

    // split signals
    assign s_real                           = {{SIGN_EXTENSION{S_AXIS_tdata[(AXIS_TDATA_WIDTH/2)-1]}}, S_AXIS_tdata[(AXIS_TDATA_WIDTH/2)-1:0]};
    assign s_imag                           = {{SIGN_EXTENSION{S_AXIS_tdata[AXIS_TDATA_WIDTH-1]}}, S_AXIS_tdata[AXIS_TDATA_WIDTH-1:AXIS_TDATA_WIDTH/2]};
    assign b_real                           = bram_portb_rddata[(BRAM_DATA_WIDTH/2)-1:0];
    assign b_imag                           = bram_portb_rddata[BRAM_DATA_WIDTH-1:BRAM_DATA_WIDTH/2];

    // S_AXIS
    assign S_AXIS_tready                    = write_enable;

    // M_AXIS
    assign M_AXIS_tvalid                    = S_AXIS_tvalid && (state == first) && aresetn;
    assign M_AXIS_tdata                     = {truncate($signed(b_imag) >>> log_count), truncate($signed(b_real) >>> log_count)};
    assign M_AXIS_tlast                     = t_last;

    // BRAM port A (write)
    assign bram_porta_addr                  = a_addr;
    assign bram_porta_clk                   = aclk;
    assign bram_porta_wrdata                = state == first ? {s_imag, s_real} : {b_imag + s_imag, b_real + s_real};
    assign bram_porta_we                    = write_enable;

    // BRAM port B (read)
    assign bram_portb_addr                  = b_addr;
    assign bram_portb_clk                   = aclk;
    assign bram_portb_en                    = write_enable;

    always @(posedge aclk) begin
        if (~aresetn) begin
            avg_count           <= 0;
            state               <= first;
            a_addr              <= 0;
            b_addr              <= 2;
            t_last              <= 0;
        end else begin
            avg_count           <= avg_count_next;
            state               <= state_next;
            a_addr              <= a_addr_next;
            b_addr              <= b_addr_next;
            t_last              <= t_last_next;
        end
    end

    always @* begin
        avg_count_next          = avg_count;
        state_next              = state;
        a_addr_next             = a_addr;
        b_addr_next             = b_addr;
        t_last_next             = 0;

        if (write_enable) begin
            a_addr_next         = a_addr + 1;
            b_addr_next         = b_addr + 1;
        end

        if (write_enable && &a_addr) begin
            if (avg_count >= max_count - 1) begin
                avg_count_next  = 0;
                state_next      = first;
            end else begin
                avg_count_next  = avg_count + 1;
                state_next      = measure;
            end
        end

        if (state == first && &a_addr_next) begin
            t_last_next = 1'b1;
        end
    end

endmodule