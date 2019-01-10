
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
    output wire                         s_axis_tready,
    input  wire [AXIS_TDATA_WIDTH-1:0]  s_axis_tdata,
    input  wire                         s_axis_tvalid,

    // master side
    output wire [AXI_ID_WIDTH-1:0]      m_axi_awid,
    output wire [ADDR_WIDTH-1:0]        m_axi_awaddr,
    output wire [7:0]                   m_axi_awlen,
    output wire [2:0]                   m_axi_awsize,
    output wire [1:0]                   m_axi_awburst,
    output wire [3:0]                   m_axi_awcache,
    output wire [2:0]                   m_axi_awprot,
    output wire [3:0]                   m_axi_awuser,
    output wire                         m_axi_awvalid,
    input  wire                         m_axi_awready,

    output wire [AXI_DATA_WIDTH-1:0]    m_axi_wdata,
    output wire [AXI_DATA_WIDTH/8-1:0]  m_axi_wstrb,
    output wire                         m_axi_wlast,
    output wire                         m_axi_wvalid,
    input  wire                         m_axi_wready,

    input  wire [1:0]                   m_axi_bresp,
    input  wire                         m_axi_bvalid,
    output wire                         m_axi_bready
);

    localparam [7:0]                    burst_size              = 16;

    reg  [3:0]                          count,                  count_next;
    reg                                 awvalid,                awvalid_next;
    reg                                 wvalid,                 wvalid_next;

    wire                                full;              
    wire                                empty;
    wire                                wlast;
    wire                                tready;
    wire [72:0]                         wdata;
    wire [72:0]                         tdata;

    assign reading                      = m_axi_wready & wvalid;
    assign writing                      = s_axis_tvalid & tready;

    assign tready                       = ~full;
    assign wlast                        = count % (burst_size - 1) == 0;
    assign tdata                        = {{(72 - AXIS_TDATA_WIDTH - ADDR_WIDTH){1'b0}}, s_axis_tdata, address};
    assign s_axis_tready                = tready;

    assign m_axi_awid                   = {(AXI_ID_WIDTH){1'b1}};                       // Write address ID
    assign m_axi_awaddr                 = wdata[ADDR_WIDTH-1:0];                        // Write address
    assign m_axi_awlen                  = burst_size;                                   // Write burst length   // AXI 3: max = 16
    assign m_axi_awsize                 = AXI_DATA_WIDTH;                               // Write burst size
    assign m_axi_awburst                = 2'b01;                                        // Write burst type     // 01b = INCR - Incrementing address
    assign m_axi_awcache                = 4'b0011;                                      // Write cache 
    assign m_axi_awprot                 = 3'b000;                                       // Write protection
    assign m_axi_awuser                 = 4'b0000;                                      // Write user data
    assign m_axi_awvalid                = awvalid;                                      // Write address valid

    assign m_axi_wdata                  = wdata[AXI_DATA_WIDTH + ADDR_WIDTH - 1:ADDR_WIDTH]; // Write data
    assign m_axi_wstrb                  = {(AXI_DATA_WIDTH/8){1'b1}};                   // Write strobes
    assign m_axi_wlast                  = wlast;                                        // Write last
    assign m_axi_wvalid                 = wvalid;                                       // Write valid

    assign m_axi_bready                 = 1'b1;                                         // Write response ready

    FIFO36E1 #(
        .FIRST_WORD_FALL_THROUGH("TRUE"),
        .ALMOST_EMPTY_OFFSET( { {(13 - 7){1'b0}}, burst_size} ),                                
        .DATA_WIDTH(72),                                                                // 512K depth
        .FIFO_MODE("FIFO36_72")
    ) fifo_0 (
        .FULL(full),
        .ALMOSTEMPTY(empty),
        .RST(~aresetn),
        .WRCLK(aclk),
        .WREN(writing),
        .DI(tdata),
        .RDCLK(aclk),
        .RDEN(reading),
        .DO(wdata)
    );

    always @(posedge aclk) begin
        if(~aresetn) begin
            count           <= 0;
            awvalid         <= 0;
            wvalid          <= 0;
        end
        else begin
            count           <= count_next;
            awvalid         <= awvalid_next;
            wvalid          <= wvalid_next;
        end
    end

    always @* begin
        count_next          = count;
        awvalid_next        = awvalid;
        wvalid_next         = wvalid;

        // - if data available but no data and address transaction is running, be optimistic and try enabling transaction of both
        if(~empty & ~awvalid & ~wvalid) begin
            awvalid_next    = 1;
            wvalid_next     = 1;
        end

        // - but if address transaction was already successful a few beats ago, i.e. burst is already running, disable it again
        if(m_axi_awready & awvalid) begin
            awvalid_next    = 0;
        end

        // - if transaction is allowed and the connected slave is ready to receive data, increase counter (address offset)
        if (reading) begin
            count_next      = count + 1;
        end

        // - allow transaction until burst_size beats are reached, then disable data transaction and enable address transaction
        if (m_axi_wready & wlast) begin
            if(empty)
                wvalid_next = 0;
            else
                awvalid_next = 1;
        end
    end

endmodule
