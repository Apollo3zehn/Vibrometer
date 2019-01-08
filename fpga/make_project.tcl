variable location [file dirname [file normalize [info script]]]

if { $argc != 1 } {
    puts "The make_project script requires a project name."
    puts "Please try again."
} else {
    set project_name [lindex $argv 0]
    set origin_dir_loc $location/$project_name

    file delete -force ./../artifacts/fpga/$project_name
    file mkdir ./../artifacts/fpga

    cd ./../artifacts/fpga

    source $location/$project_name/block_design.tcl

    make_wrapper -files [get_files $location/$project_name/system.bd] -top
    add_files -norecurse $location/$project_name/system_wrapper.v
}
