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
    input  wire [MM_ADDR_WIDTH-1:0]         SM_address,
    output wire [MM_ADDR_WIDTH-1:0]         SM_read_buffer,
    
    // axis master
    input  wire                             M_AXIS_tready,
    output wire                             M_AXIS_tvalid,
    output wire [MM_ADDR_WIDTH+47-8:0]      M_AXIS_tdata
);

    localparam                              buffer_1            = 2'b00, 
                                            buffer_2            = 2'b01,
                                            buffer_3            = 2'b10;

    reg  [1:0]                              state_write,        state_write_next;
    reg  [1:0]                              state_full,         state_full_next;
    reg  [MM_ADDR_WIDTH-1:0]                write,              write_next;
    reg  [MM_ADDR_WIDTH-1:0]                read,               read_next;
    reg  [MM_ADDR_WIDTH-1:0]                count,              count_next;
    reg                                     tvalid,             tvalid_next;
    reg                                     lock,               lock_next;
    
    wire [22:0]                             length;

    assign SM_read_buffer                   = read;
    assign M_AXIS_tvalid                    = 1'b1;
    assign length                           = 1 << SM_log_length;

    assign M_AXIS_tdata                     = {
                                                // 4'b0,              // xCACHE
                                                // 4'b0,              // xUSER
                                                4'b0,              // RSVD
                                                4'b0,              // TAG
                                                write,             // ADDR
                                                1'b0,              // DRR
                                                1'b0,              // EOF
                                                6'b0,              // DSA
                                                1'b1,              // Type
                                                length             // BTT
                                               };

    always @(posedge SYS_aclk) begin
            if (~SYS_aresetn) begin
                state_write     <= buffer_1;
                state_full      <= buffer_3;
                write           <= 0;
                read            <= 0;
                count           <= 0;
                tvalid          <= 0;
                lock            <= 0;
            end
            else begin
                state_write     <= state_write_next;
                state_full      <= state_full_next;
                write           <= write_next;
                read            <= read_next;
                count           <= count_next;
                tvalid          <= tvalid_next;
                lock            <= lock_next;
            end
        end
      
        always @* begin
            state_write_next    = state_write;
            state_full_next     = state_full;
            write_next          = write;
            read_next           = read;
            count_next          = count + 1;
            tvalid_next         = tvalid;
            lock_next           = SM_request;
        
            if (SM_request) begin
                if (~lock) begin
                    if (state_full == buffer_1)
                        read_next = SM_address + length * 0;
                    else if (state_full == buffer_2)
                        read_next = SM_address + length * 1;
                    else if (state_full == buffer_3)
                        read_next = SM_address + length * 2;
                    else
                        read_next = 0;
                end
            end
            else begin
                read_next       = 0;
            end            
            
            if (count == length - 1)
                count_next = 0;
        
            case(state_write)
                       
                buffer_1: begin                   
                    if (count == length - 1) begin
                        if (state_full == buffer_3) begin
                            state_write_next = buffer_2;
                            write_next = SM_address + length * 1;
                        end
                        else begin
                            state_write_next = buffer_3;
                            write_next = SM_address + length * 2;
                        end

                        state_full_next = buffer_1;                   
                        tvalid_next = 1;  
                    end
                    else begin
                        tvalid_next = 0; 
                    end
                end
                   
                buffer_2: begin
                    if (count == length - 1) begin
                        if (state_full == buffer_3) begin
                            state_write_next = buffer_1;
                            write_next = SM_address + length * 0;
                        end
                        else begin
                            state_write_next = buffer_3;
                            write_next = SM_address + length * 2;
                        end
                        
                        state_full_next = buffer_2;                             
                        tvalid_next = 1;   
                    end
                    else begin
                        tvalid_next = 0; 
                    end                
                end
                
                buffer_3: begin
                   if (count == length - 1) begin
                        if (state_full == buffer_1) begin
                            state_write_next = buffer_2;
                            write_next = SM_address + length * 1;
                        end
                        else begin
                            state_write_next = buffer_1;
                            write_next = SM_address + length * 0;
                        end
                            
                        state_full_next = buffer_3;
                        tvalid_next = 1;  
                    end
                    else begin
                        tvalid_next = 0; 
                    end             
                end
                                 
            endcase       
        end

endmodule
