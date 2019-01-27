set display_name {AXI4-Stream Complex Averager}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter AXIS_TDATA_WIDTH {AXIS TDATA WIDTH}  {Width of the AXI stream data port.}
core_parameter BRAM_DATA_WIDTH  {BRAM DATA WIDTH}   {Width of the BRAM data port.}
core_parameter BRAM_ADDR_WIDTH  {BRAM ADDR WIDTH}   {Width of the BRAM address port.}

# S_AXIS
set bus [ipx::get_bus_interfaces -of_objects $core S_AXIS]
set_property NAME S_AXIS $bus
set_property INTERFACE_MODE slave $bus

# M_AXIS
set bus [ipx::get_bus_interfaces -of_objects $core M_AXIS]
set_property NAME M_AXIS $bus
set_property INTERFACE_MODE master $bus

# aclk
set bus [ipx::get_bus_interfaces aclk]
set parameter [ipx::get_bus_parameters -of_objects $bus ASSOCIATED_BUSIF]
set_property VALUE S_AXIS:M_AXIS $parameter

# BRAM port A
set bus [ipx::add_bus_interface BRAM_PORTA $core]
set_property ABSTRACTION_TYPE_VLNV xilinx.com:interface:bram_rtl:1.0 $bus
set_property BUS_TYPE_VLNV xilinx.com:interface:bram:1.0 $bus
set_property INTERFACE_MODE master $bus
foreach {logical physical} {
  CLK  bram_porta_clk
  ADDR bram_porta_addr
  DIN  bram_porta_wrdata
  WE   bram_porta_we
} {
  set_property PHYSICAL_NAME $physical [ipx::add_port_map $logical $bus]
}

set bus [ipx::get_bus_interfaces bram_porta_clk]
set parameter [ipx::add_bus_parameter ASSOCIATED_BUSIF $bus]
set_property VALUE BRAM_PORTA $parameter

# BRAM port B
set bus [ipx::add_bus_interface BRAM_PORTB $core]
set_property ABSTRACTION_TYPE_VLNV xilinx.com:interface:bram_rtl:1.0 $bus
set_property BUS_TYPE_VLNV xilinx.com:interface:bram:1.0 $bus
set_property INTERFACE_MODE master $bus
foreach {logical physical} {
  CLK  bram_portb_clk
  ADDR bram_portb_addr
  DOUT bram_portb_rddata
} {
  set_property PHYSICAL_NAME $physical [ipx::add_port_map $logical $bus]
}

set bus [ipx::get_bus_interfaces bram_portb_clk]
set parameter [ipx::add_bus_parameter ASSOCIATED_BUSIF $bus]
set_property VALUE BRAM_PORTB $parameter
