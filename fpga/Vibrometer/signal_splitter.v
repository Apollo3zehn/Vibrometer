module signal_splitter # 
(
    parameter ADC_DATA_WIDTH = 16,
    parameter AXIS_TDATA_WIDTH = 32
)
(
    (* X_INTERFACE_PARAMETER = "FREQ_HZ 125000000" *)
    input [AXIS_TDATA_WIDTH-1:0]        S_AXIS_tdata,
    input                               S_AXIS_tvalid,
    (* X_INTERFACE_PARAMETER = "FREQ_HZ 125000000" *)
    output wire [AXIS_TDATA_WIDTH-1:0]  M_AXIS_PORT1_tdata,
    output wire                         M_AXIS_PORT1_tvalid,
    (* X_INTERFACE_PARAMETER = "FREQ_HZ 125000000" *)
    output wire [AXIS_TDATA_WIDTH-1:0]  M_AXIS_PORT2_tdata,
    output wire                         M_AXIS_PORT2_tvalid
);
        
    assign M_AXIS_PORT1_tdata = {{(AXIS_TDATA_WIDTH-ADC_DATA_WIDTH+1){S_AXIS_tdata[ADC_DATA_WIDTH-1]}},S_AXIS_tdata[ADC_DATA_WIDTH-1:0]};
    assign M_AXIS_PORT2_tdata = {{(AXIS_TDATA_WIDTH-ADC_DATA_WIDTH+1){S_AXIS_tdata[AXIS_TDATA_WIDTH-1]}},S_AXIS_tdata[AXIS_TDATA_WIDTH-1:ADC_DATA_WIDTH]};
    assign M_AXIS_PORT1_tvalid = S_AXIS_tvalid;
    assign M_AXIS_PORT2_tvalid = S_AXIS_tvalid;

endmodule