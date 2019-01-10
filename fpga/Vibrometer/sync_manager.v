`timescale 1ns / 1ps

module sync_manager #
(
    parameter integer                       MM_ADDR_WIDTH       = 32
)
(
    // system signals
    input  wire                             SYS_aclk,
    input  wire                             SYS_aresetn,
    
    // SM signals
    input  wire                             SM_request,
    input  wire [4:0]                       SM_log_length,
    input  wire [MM_ADDR_WIDTH-1:0]         SM_base_address,
    input  wire                             SM_reading,
    input  wire                             SM_writing,
    output wire [MM_ADDR_WIDTH-1:0]         SM_read_buffer,
    output wire [MM_ADDR_WIDTH-1:0]         SM_write_buffer
);

    localparam                              buffer_1            = 2'b00, 
                                            buffer_2            = 2'b01,
                                            buffer_3            = 2'b10,
                                            buffer_4            = 2'b11;

    reg  [1:0]                              state_read,         state_read_next;
    reg  [1:0]                              state_ready,        state_ready_next;
    reg  [1:0]                              state_lock,         state_lock_next;
    reg  [1:0]                              state_write,        state_write_next;

    reg  [MM_ADDR_WIDTH-1:0]                address_write,      address_write_next;
    reg  [MM_ADDR_WIDTH-1:0]                address_read,       address_read_next;
    reg  [MM_ADDR_WIDTH-1:0]                read_count,         read_count_next;
    reg  [MM_ADDR_WIDTH-1:0]                write_count,        write_count_next;
    reg                                     lock,               lock_next;
    
    wire [22:0]                             length;

    assign SM_read_buffer                   = address_read;
    assign SM_write_buffer                  = address_write;
    assign length                           = 1 << SM_log_length;

    always @(posedge SYS_aclk) begin
        if (~SYS_aresetn) begin
            state_read      <= buffer_1;
            state_ready     <= buffer_2;
            state_lock      <= buffer_3;
            state_write     <= buffer_4;
            address_read    <= 0;
            address_write   <= 0;
            read_count      <= 0;
            write_count     <= 0;
            lock            <= 0;
        end
        else begin
            state_read      <= state_read_next;
            state_ready     <= state_ready_next;
            state_lock      <= state_lock_next;
            state_write     <= state_write_next;
            address_read    <= address_read_next;
            address_write   <= address_write_next;
            read_count      <= read_count_next;
            write_count     <= write_count_next;
            lock            <= lock_next;
        end
    end

    always @* begin
        lock_next                   = SM_request;
        state_read_next             = state_read;
        address_read_next           = address_read;
    
        if (SM_request) begin
            if (~lock) begin
                state_read_next     = state_ready;
                address_read_next   = SM_base_address + length * state_read_next;
            end
        end
        else begin
            address_read_next       = 0;
        end            
    end

    always @* begin
        read_count_next         = read_count;
        write_count_next        = write_count;
        state_write_next        = state_write;
        state_lock_next         = state_lock;
        state_ready_next        = state_ready;
        address_write_next      = address_write;
        
        // if s2mm read transfer was successful, increase read_count
        if (SM_reading) begin
            read_count_next     = read_count + 1;
        end

        // if read_count has reached the maximum, assign a new buffer
        if (read_count >= length - 1) begin
            read_count_next     = 0;

            if (state_write + 1 == state_read)
                state_write_next = state_write + 2;
            else
                state_write_next = state_write + 1;
        end

        // if s2mm write transfer was successful, increase write_count
        if (SM_writing) begin
            write_count_next    = write_count + 1;
        end

        // if write_count has reached the maximum, assign new buffers
        if (write_count >= length - 1) begin
            write_count_next    = 0;

            if (state_lock + 1 == state_read)
                state_lock_next = state_lock + 2;
            else
                state_lock_next = state_lock + 1;
                
            if (state_ready + 1 == state_read)
                state_ready_next = state_ready + 2;
            else
                state_ready_next = state_ready + 1;
        end

        address_write_next      = SM_base_address + length * state_write_next;
    end

endmodule
