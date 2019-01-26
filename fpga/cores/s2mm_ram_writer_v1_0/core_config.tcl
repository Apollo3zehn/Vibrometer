set display_name {Stream to Memory-Map Ram Writer}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter AXI_ADDR_WIDTH   {AXI address width} {Width of the AXI address port.}
core_parameter AXI_ID_WIDTH     {AXI ID width}      {Width of the AXI ID port.}
core_parameter AXI_DATA_WIDTH   {AXI data width}    {Width of the AXI data port.}
core_parameter AXIS_TDATA_WIDTH {AXIS tdata width}  {Width of the AXI stream data port.}

# S_AXIS
set bus [ipx::get_bus_interfaces -of_objects $core S_AXIS]
set_property NAME S_AXIS $bus
set_property INTERFACE_MODE slave $bus

# M_AXI
set bus [ipx::get_bus_interfaces -of_objects $core M_AXI]
set_property NAME M_AXI $bus
set_property INTERFACE_MODE master $bus

# aclk
set bus [ipx::get_bus_interfaces aclk]
set parameter [ipx::get_bus_parameters -of_objects $bus ASSOCIATED_BUSIF]
set_property VALUE S_AXIS:M_AXI $parameter