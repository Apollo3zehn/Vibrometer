set display_name {AXI4-Stream Data Width Adapter}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter AXIS_TDATA_WIDTH_IN  {AXIS TDATA WIDTH (Input)}  {Width of the input AXI stream data port.}
core_parameter AXIS_TDATA_WIDTH_OUT {AXIS TDATA WIDTH (Output)} {Width of the output AXI stream data port.}

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