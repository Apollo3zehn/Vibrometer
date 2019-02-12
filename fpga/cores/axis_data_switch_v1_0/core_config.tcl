set display_name {AXI4-Stream Data Switch}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter AXIS_TDATA_WIDTH {AXIS TDATA WIDTH}  {Width of the AXI stream data ports.}
