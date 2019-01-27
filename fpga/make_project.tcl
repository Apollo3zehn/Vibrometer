variable location [file dirname [file normalize [info script]]]

set project_name [lindex $argv 0]
set origin_dir_loc $location/$project_name

file delete -force ./../artifacts/fpga/$project_name
file mkdir ./../artifacts/fpga

cd ./../artifacts/fpga

source $location/$project_name/block_design.tcl

make_wrapper -files [get_files $location/$project_name/system.bd] -top
add_files -norecurse $location/$project_name/system_wrapper.v
