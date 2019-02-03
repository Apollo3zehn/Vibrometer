set display_name {Frequency Modulation Manager}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter CARRIER_PINC_WIDTH  {Carrier Phase Increment Width} {Width of the carrier phase increment.}
core_parameter SIGNAL_PHASE_WIDTH  {Signal Phase Width}            {Width of the signal phase.}
core_parameter S_AXIS_TDATA_WIDTH  {Slave AXIS tdata width}        {Width of the AXI slave stream data port.}
core_parameter M_AXIS_TDATA_WIDTH  {Master AXIS tdata width}       {Width of the AXI master stream data port.}

# S_AXIS
set bus [ipx::get_bus_interfaces -of_objects $core S_AXIS]
set_property NAME S_AXIS $bus
set_property INTERFACE_MODE slave $bus

# M_AXI
set bus [ipx::get_bus_interfaces -of_objects $core M_AXIS]
set_property NAME M_AXIS $bus
set_property INTERFACE_MODE master $bus

# aclk
set bus [ipx::get_bus_interfaces aclk]
set parameter [ipx::get_bus_parameters -of_objects $bus ASSOCIATED_BUSIF]
set_property VALUE S_AXIS:M_AXIS $parameter