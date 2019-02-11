set display_name {Sync Manager}

set core [ipx::current_core]

set_property DISPLAY_NAME $display_name $core
set_property DESCRIPTION $display_name $core

core_parameter MM_ADDR_WIDTH {Memory-Map Address Width} {Width of the address channel.}
core_parameter DATA_WIDTH    {Data Width}               {Width of the data channel.}