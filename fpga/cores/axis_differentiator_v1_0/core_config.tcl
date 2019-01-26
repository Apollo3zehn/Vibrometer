set display_name {AXIS Differentiator}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter AXIS_TDATA_WIDTH {AXIS TDATA WIDTH} {Width of the AXI stream data port.}

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
