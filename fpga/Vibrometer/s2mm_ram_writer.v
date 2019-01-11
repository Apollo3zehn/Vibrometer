
`timescale 1 ns / 1 ps

module s2mm_ram_writer #
(
    parameter integer ADDR_WIDTH        = 32,
    parameter integer AXI_ID_WIDTH      = 6,
    parameter integer AXI_DATA_WIDTH    = 32,
    parameter integer AXIS_TDATA_WIDTH  = 32
)
(
    // system signals
    input  wire                         aclk,
    input  wire                         aresetn,

    input  wire [ADDR_WIDTH-1:0]        address,
    output wire                         reading,
    output wire                         writing,

    // slave side
    output wire                         S_AXIS_tready,
    input  wire [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata,
    input  wire                         S_AXIS_tvalid,

    // master side
    output wire [AXI_ID_WIDTH-1:0]      M_AXI_awid,
    output wire [ADDR_WIDTH-1:0]        M_AXI_awaddr,
    output wire [7:0]                   M_AXI_awlen,
    output wire [2:0]                   M_AXI_awsize,
    output wire [1:0]                   M_AXI_awburst,
    output wire [3:0]                   M_AXI_awcache,
    output wire [2:0]                   M_AXI_awprot,
    output wire [3:0]                   M_AXI_awuser,
    output wire                         M_AXI_awvalid,
    input  wire                         M_AXI_awready,

    output wire [AXI_DATA_WIDTH-1:0]    M_AXI_wdata,
    output wire [AXI_DATA_WIDTH/8-1:0]  M_AXI_wstrb,
    output wire                         M_AXI_wlast,
    output wire                         M_AXI_wvalid,
    input  wire                         M_AXI_wready,

    input  wire [1:0]                   M_AXI_bresp,
    input  wire                         M_AXI_bvalid,
    output wire                         M_AXI_bready
);

    localparam [7:0]                    burst_size              = 16;

    reg  [3:0]                          count,                  count_next;
    reg  [2:0]                          count_rst,              count_rst_next;
    reg                                 awvalid,                awvalid_next;
    reg                                 wvalid,                 wvalid_next;

    wire                                full;              
    wire                                empty;
    wire                                wlast;
    wire                                tready;
    wire [63:0]                         wdata;
    wire [63:0]                         tdata;
    wire [12:0]                         rdcount;
    wire [12:0]                         wrcount;

    function integer clogb2 (input integer value);
        for(clogb2 = 0; value > 0; clogb2 = clogb2 + 1) begin
            value = value >> 1;
        end
    endfunction

    localparam integer ADDR_SIZE = clogb2((AXI_DATA_WIDTH/8)-1);

    // TODO:
    // The write operation is synchronous, writing the data word available at DI into the FIFO whenever WREN is active one setup time before the rising WRCLK edge.
    // The read operation is also synchronous, presenting the next data word at DO whenever the RDEN is active one setup time before the rising RDCLK edge.

    // https://www.xilinx.com/support/documentation/user_guides/ug473_7Series_Memory_Resources.pdf 48 pp.
    // Note: "writing" is "RDEN" and "reading" is "WREN" for FIFO!
    // ===========================================================
    // TODO: clear register during reset
    // TODO: RST must be held high for at least three RDCLK clock cycles, and RDEN must be low for four clock cycles before RST becomes active high, and RDEN remains low during this reset cycle.
    // DONE: RDEN must be held low during reset cycle
    // DONE: RDEN must be low for at least two RDCLK clock cycles after RST deasserted.
    assign writing                      = M_AXI_wready  & wvalid & count_rst > 2;
    // TODO: RST must be held high for at least three WRCLK clock cycles, and WREN must be low for four clock cycles before RST becomes active high, and WREN remains low during this reset cycle.
    // DONE: WREN must be held low during reset cycle
    // DONE: WREN must be low for at least two WRCLK clock cycles after RST deasserted.
    assign reading                      = S_AXIS_tvalid & tready & count_rst > 2;

    assign tready                       = ~full;
    assign wlast                        = &count;
    assign tdata                        = {{(64 - AXIS_TDATA_WIDTH - ADDR_WIDTH){1'b0}}, S_AXIS_tdata, address};
    assign S_AXIS_tready                = tready;

    assign M_AXI_awid                   = {(AXI_ID_WIDTH){1'b1}};                       // Write address ID
    assign M_AXI_awaddr                 = wdata[ADDR_WIDTH-1:0];                        // Write address
    assign M_AXI_awlen                  = burst_size;                                   // Write burst length   // AXI 3: max = 16
    assign M_AXI_awsize                 = ADDR_SIZE;                                    // Write burst size     // # of bytes = 2^awsize
    assign M_AXI_awburst                = 2'b01;                                        // Write burst type     // 01b = INCR - Incrementing address
    assign M_AXI_awcache                = 4'b0011;                                      // Write cache 
    assign M_AXI_awprot                 = 3'b000;                                       // Write protection
    assign M_AXI_awuser                 = 4'b0000;                                      // Write user data
    assign M_AXI_awvalid                = awvalid;                                      // Write address valid

    assign M_AXI_wdata                  = wdata[AXI_DATA_WIDTH + ADDR_WIDTH - 1:ADDR_WIDTH]; // Write data
    assign M_AXI_wstrb                  = {(AXI_DATA_WIDTH/8){1'b1}};                   // Write strobes
    assign M_AXI_wlast                  = wlast;                                        // Write last
    assign M_AXI_wvalid                 = wvalid;                                       // Write valid

    assign M_AXI_bready                 = 1'b1;                                         // Write response ready

    FIFO36E1 #(
        .ALMOST_EMPTY_OFFSET( { {(13 - 8){1'b0}}, burst_size} ),                        // 8 = sizeof(burst_size)
        .DATA_WIDTH(72),                                                                // 512K depth
        .EN_SYN("TRUE"),                                                                // synchronuous clocks
        .DO_REG(0),                                                                     // synchronuous clocks
        .FIFO_MODE("FIFO36_72")
    ) fifo_0 (
        .RST(~aresetn),
        .DI(tdata),
        .DO(wdata),
        .WREN(reading),                                                                 // "reading" means that data are read from module input stream (i.e FIFO is allowed to write)
        .RDEN(writing),                                                                 // "writing" means that data are written into memory (i.e FIFO is allowd to read)
        .WRCLK(aclk),
        .RDCLK(aclk),
        .WRCOUNT(wrcount),
        .RDCOUNT(rdcount),
        .FULL(full),
        .ALMOSTEMPTY(empty)
    );

    always @(posedge aclk) begin
        if(~aresetn) begin
            count           <= 0;
            count_rst       <= 0;
            awvalid         <= 0;
            wvalid          <= 0;
        end
        else begin
            count           <= count_next;
            count_rst       <= count_rst_next;
            awvalid         <= awvalid_next;
            wvalid          <= wvalid_next;
        end
    end

    always @* begin
        count_next          = count;
        count_rst_next      = count_rst;
        awvalid_next        = awvalid;
        wvalid_next         = wvalid;

        // for FIFO
        if (count_rst <= 2)
            count_rst_next  = count_rst + 1;

        // - if data available but no data and address transaction is running, be optimistic and try enabling transaction of both
        if(~empty & ~awvalid & ~wvalid) begin
            awvalid_next    = 1;
            wvalid_next     = 1;
        end

        // - but if address transaction was already successful one beat ago, i.e. burst is already running, disable it again
        if(M_AXI_awready & awvalid) begin
            awvalid_next    = 0;
        end

        // - if transaction is allowed and the connected slave is ready to receive data, increase counter (address offset)
        if (writing) begin
            count_next      = count + 1;
        end

        // - allow transaction until burst_size beats are reached, then wait for data to arrive or otherwise restart immediately
        if (M_AXI_wready & wlast) begin
            if(empty)
                wvalid_next = 0;
            else
                awvalid_next = 1;
        end
    end

endmodule
