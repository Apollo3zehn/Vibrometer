
`timescale 1 ns / 1 ps

module s2mm_ram_writer #
(
    parameter integer AXI_ADDR_WIDTH    = 32,
    parameter integer AXI_ID_WIDTH      = 6,
    parameter integer AXI_DATA_WIDTH    = 32,
    parameter integer AXIS_TDATA_WIDTH  = 32
)
(
    // system signals
    input  wire                         aclk,
    input  wire                         aresetn,

    input  wire [AXI_ADDR_WIDTH-1:0]    address,
    output wire                         reading,
    output wire                         writing,

    // slave side
    output wire                         S_AXIS_tready,
    input  wire [AXIS_TDATA_WIDTH-1:0]  S_AXIS_tdata,
    input  wire                         S_AXIS_tvalid,

    // master side
    output wire [AXI_ID_WIDTH-1:0]      M_AXI_awid,
    output wire [AXI_ADDR_WIDTH-1:0]    M_AXI_awaddr,
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

    localparam                          idle                    = 2'b00, 
                                        addressing              = 2'b01,
                                        data                    = 2'b10;

    localparam  [7:0]                   burst_length            = 16;
    localparam integer                  ADDR_SIZE               = clogb2((AXI_DATA_WIDTH/8)-1);

    reg  [3:0]                          count,                  count_next;
    reg  [2:0]                          count_rst,              count_rst_next;
    reg                                 awvalid,                awvalid_next;
    reg                                 wvalid,                 wvalid_next;
    reg  [1:0]                          state,                  state_next;

    wire                                full;              
    wire                                empty;
    wire                                wlast;
    wire [63:0]                         wdata;
    wire [63:0]                         tdata;
    wire [12:0]                         rdcount;
    wire [12:0]                         wrcount;

    function integer clogb2 (input integer value);
        for(clogb2 = 0; value > 0; clogb2 = clogb2 + 1) begin
            value = value >> 1;
        end
    endfunction


    // https://www.xilinx.com/support/documentation/user_guides/ug473_7Series_Memory_Resources.pdf 48 pp.
    // Note: "writing" is "RDEN" and "reading" is "WREN" for FIFO!
    // TODO: wait for bresp?

    // TODO: RST must be held high for at least five RDCLK clock cycles, ...
    // DONE: ... and RDEN must be low before RST becomes active high, and RDEN remains low during this reset cycle.
    // DONE: The read operation is also synchronous, presenting the next data word at DO whenever the RDEN is active one setup time before the rising RDCLK edge.
    // DONE: RDEN must be held low during reset cycle
    // DONE: RDEN must be low for at least two RDCLK clock cycles after RST deasserted.
    assign writing                      = wvalid        & M_AXI_wready  & aresetn & count_rst > 2;
    // TODO: RST must be held high for at least five WRCLK clock cycles ...
    // DONE: ... and WREN must be low before RST becomes active high, and WREN remains low during this reset cycle.
    // DONE: The write operation is synchronous, writing the data word available at DI into the FIFO whenever WREN is active one setup time before the rising WRCLK edge.
    // DONE: WREN must be held low during reset cycle
    // DONE: WREN must be low for at least two WRCLK clock cycles after RST deasserted.
    assign reading                      = S_AXIS_tvalid & S_AXIS_tready & aresetn & count_rst > 2;

    assign wlast                        = &count;
    assign tdata                        = {{(64 - AXIS_TDATA_WIDTH - AXI_ADDR_WIDTH){1'b0}}, S_AXIS_tdata, address};
    assign S_AXIS_tready                = ~full;

    assign M_AXI_awid                   = {(AXI_ID_WIDTH){1'b0}};                       // Write address ID
    assign M_AXI_awaddr                 = wdata[AXI_ADDR_WIDTH-1:0];                    // Write address
    assign M_AXI_awlen                  = burst_length - 1;                             // Write burst length   // AXI 3: max = 16, burst_length = awlen[7:0] + 1
    assign M_AXI_awsize                 = ADDR_SIZE;                                    // Write burst size     // # of bytes = 2^awsize
    assign M_AXI_awburst                = 2'b01;                                        // Write burst type     // 01b = INCR - Incrementing address
    assign M_AXI_awcache                = 4'b0011;                                      // Write cache 
    assign M_AXI_awprot                 = 3'b000;                                       // Write protection
    assign M_AXI_awuser                 = 4'b0000;                                      // Write user data
    assign M_AXI_awvalid                = awvalid;                                      // Write address valid

    assign M_AXI_wdata                  = wdata[AXI_DATA_WIDTH + AXI_ADDR_WIDTH - 1:AXI_ADDR_WIDTH]; // Write data
    assign M_AXI_wstrb                  = {(AXI_DATA_WIDTH/8){1'b1}};                   // Write strobes
    assign M_AXI_wlast                  = wlast;                                        // Write last
    assign M_AXI_wvalid                 = wvalid;                                       // Write valid

    assign M_AXI_bready                 = 1'b1;                                         // Write response ready

    FIFO36E1 #(
        .FIRST_WORD_FALL_THROUGH("TRUE"),
        .ALMOST_EMPTY_OFFSET( { {(13 - 8){1'b0}}, burst_length} ),                      // 8 = sizeof(burst_length)
        .DATA_WIDTH(72),                                                                // 512K depth
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
            count_rst       <= 0;
            awvalid         <= 0;

            // allow burst to finish during reset
            if (state > idle) begin
                count       <= count_next;
                wvalid      <= wvalid_next;
                state       <= state_next;
            end else begin
                count       <= 0;
                wvalid      <= 0;
                state       <= idle;
            end
        end else begin
            count           <= count_next;
            count_rst       <= count_rst_next;
            awvalid         <= awvalid_next;
            wvalid          <= wvalid_next;
            state           <= state_next;
        end
    end

    always @* begin
        count_next          = count;
        count_rst_next      = count_rst;
        awvalid_next        = awvalid;
        wvalid_next         = wvalid;
        state_next          = state;

        // FIFO
        if (count_rst <= 2)
            count_rst_next  = count_rst + 1;

        // keep track of burst position
        if (writing || ~aresetn) begin
            count_next      = count + 1;
        end

        case(state)
        
            idle: begin
                if(~empty) begin
                    awvalid_next    = 1'b1;
                    wvalid_next     = 1'b1;
                    state_next      = addressing;
                end
            end
            
            addressing: begin
                if (M_AXI_awready) begin
                    awvalid_next    = 0;
                    state_next      = data;
                end
            end

            data: begin           
                if (wlast) begin
                    wvalid_next     = 0;
                    state_next      = idle;
                end
            end
                            
        endcase
    end

endmodule
