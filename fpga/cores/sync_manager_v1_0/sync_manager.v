`timescale 1ns / 1ps

module sync_manager #
(
    parameter integer                       MM_ADDR_WIDTH       = 32,
    parameter integer                       DATA_WIDTH          = 32
)
(
    // system signals
    input  wire                             aclk,
    input  wire                             aresetn,
    output wire [3:0]                       combination,
    
    // SM signals
    input  wire                             SM_request,
    input  wire [4:0]                       SM_log_length,
    input  wire [MM_ADDR_WIDTH-1:0]         SM_base_address,
    input  wire                             SM_reading,
    input  wire                             SM_writing,
    output wire [MM_ADDR_WIDTH-1:0]         SM_read_buffer,
    output wire [MM_ADDR_WIDTH-1:0]         SM_write_buffer
);

    function [MM_ADDR_WIDTH-1:0] buffer_to_factor (input [3:0] value);
        begin
            if (value[0])
                buffer_to_factor = 0;
            else if (value[1])
                buffer_to_factor = 1;
            else if (value[2])
                buffer_to_factor = 2;
            else
                buffer_to_factor = 3;
        end
    endfunction

    localparam                              buffer_1            = 4'b0001, 
                                            buffer_2            = 4'b0010,
                                            buffer_3            = 4'b0100,
                                            buffer_4            = 4'b1000;

    reg  [3:0]                              state_read,         state_read_next;
    reg  [3:0]                              state_ready,        state_ready_next;
    reg  [3:0]                              state_lock,         state_lock_next;
    reg  [3:0]                              state_write,        state_write_next;

    reg  [MM_ADDR_WIDTH-1:0]                read_count,         read_count_next;
    reg  [MM_ADDR_WIDTH-1:0]                write_count,        write_count_next;
    reg                                     lock,               lock_next;
    reg  [31:0]                             write_buffer_tmp,   write_buffer_tmp_next;

    wire [31:0]                             length;
    
    assign combination                      = state_read | state_ready | state_lock | state_write;
    assign length                           = 1 << SM_log_length;
    assign SM_read_buffer                   = SM_base_address + length * buffer_to_factor(state_read) * (DATA_WIDTH / 8);
    assign SM_write_buffer                  = write_buffer_tmp + length * buffer_to_factor(state_write) * (DATA_WIDTH / 8);

    always @(posedge aclk) begin
        if (~aresetn) begin
            state_read          <= buffer_1;
            state_ready         <= buffer_2;
            state_lock          <= buffer_3;
            state_write         <= buffer_3;
            read_count          <= 0;
            write_count         <= 0;
            lock                <= 0;
            write_buffer_tmp    <= 0;
        end else begin
            state_read          <= state_read_next;
            state_ready         <= state_ready_next;
            state_lock          <= state_lock_next;
            state_write         <= state_write_next;
            read_count          <= read_count_next;
            write_count         <= write_count_next;
            lock                <= lock_next;
            write_buffer_tmp    <= write_buffer_tmp_next;
        end
    end

    always @* begin
        lock_next               = SM_request;
        read_count_next         = read_count;
        write_count_next        = write_count;
        state_read_next         = state_read;
        state_ready_next        = state_ready;
        state_lock_next         = state_lock;
        state_write_next        = state_write;
        
        // if s2mm read transfer was successful, increase read_count
        if (SM_reading) begin
            read_count_next     = read_count + 1;
        end

        // if read_count has reached the maximum, assign a new buffer
        if (read_count_next >= length) begin
            read_count_next     = 0;

            if (combination[0] == 1'b0)
                state_write_next = buffer_1;
            else if (combination[1] == 1'b0)
                state_write_next = buffer_2;
            else if (combination[2] == 1'b0)
                state_write_next = buffer_3;
            else if (combination[3] == 1'b0)
                state_write_next = buffer_4;
            else begin
                state_write_next = state_ready;
                state_ready_next = state_read;
            end
        end

        // calculate part of the final write address
        write_buffer_tmp_next   = SM_base_address + read_count_next * (DATA_WIDTH / 8);

        // if s2mm write transfer was successful, increase write_count
        if (SM_writing) begin
            write_count_next    = write_count + 1;
        end

        // if write_count has reached the maximum, assign new buffers
        if (write_count >= length - 1) begin
            write_count_next    = 0;
            state_lock_next     = state_write;
            state_ready_next    = state_lock;
        end

        // if read request
        if (SM_request) begin
            if (~lock) begin
                state_read_next = state_ready_next;
            end
        end
    end

endmodule

// BUFFER MANAGEMENT
// ===============================
//
//
// Step 1 (initial):
// ===============================
// read         0001
// ready        0010
// lock         0100
// write        0100
// ===============================
// free         1000
//
//
// Step 2 (write changes to free):
// ===============================
// read         0001
// ready        0010
// lock         0100
// write        1000
// ===============================
// free         <none>
//
//
// Step 3 (lock & ready change):
// ===============================
// read         0001
// ready        0100
// lock         1000
// write        1000
// ===============================
// free         0010
//
//
// Step 4 (write changes to free):
// ===============================
// read         0001
// ready        0100
// lock         1000
// write        0010
// ===============================
// free         <none>
//
//
// Step 5 (lock & ready change):
// ===============================
// read         0001
// ready        1000
// lock         0010
// write        0010
// ===============================
// free         <none>
//
//
// Step 6 (read changes to ready):
// ===============================
// read         1000
// ready        1000
// lock         0010
// write        0010
// ===============================
// free         0100 and 0001
//
//
// Step 7 (write changes to free):
// ===============================
// read         1000
// ready        1000
// lock         0010
// write        0001
// ===============================
// free         0001 and 0100
//
//
// Step 8 (lock & ready change):
// ===============================
// read         1000
// ready        0010
// lock         0001
// write        0001
// ===============================
// free         0100
//
//
// Step 9 (read changes to ready):
// ===============================
// read         0010
// ready        0010
// lock         0001
// write        0001
// ===============================
// free         0100 and 1000