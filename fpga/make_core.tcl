
set partName [lindex $argv 0]
set coreName [lindex $argv 1]
set projectName [lindex $argv 2]
set version [lindex $argv 3]
set sourceDirectoryPath [lindex $argv 4]
set targetDirectoryPath [lindex $argv 5]

create_project -force -part $partName $projectName $targetDirectoryPath

set fileset_source_1 [glob -nocomplain $sourceDirectoryPath/$coreName/*.*v]
set fileset_sim_1 [glob -nocomplain $sourceDirectoryPath/$coreName/*_tb.*v]

set x {}
foreach elem $fileset_source_1 {dict set x $elem 1}
foreach elem $fileset_sim_1 {dict unset x $elem}
set fileset_source_1 [dict keys $x]

add_files -quiet -fileset sources_1   -norecurse $fileset_source_1
add_files -quiet -fileset sim_1       -norecurse $fileset_sim_1
add_files -quiet -fileset sim_1       -norecurse [glob -nocomplain $sourceDirectoryPath/$coreName/*_tb.wcfg]

ipx::package_project -import_files -root_dir $targetDirectoryPath/$coreName

set core [ipx::current_core]

set_property VERSION $version $core
set_property NAME $projectName $core
set_property LIBRARY {user} $core
set_property VENDOR {Apollo3zehn} $core
set_property VENDOR_DISPLAY_NAME {Apollo3zehn} $core
set_property COMPANY_URL {https://github.com/apollo3zehn} $core
set_property SUPPORTED_FAMILIES {zynq Production} $core

proc core_parameter {name display_name description} {
  set core [ipx::current_core]

  set parameter [ipx::get_user_parameters $name -of_objects $core]
  set_property DISPLAY_NAME $display_name $parameter
  set_property DESCRIPTION $description $parameter

  set parameter [ipgui::get_guiparamspec -name $name -component $core]
  set_property DISPLAY_NAME $display_name $parameter
  set_property TOOLTIP $description $parameter
}

source $sourceDirectoryPath/$coreName/core_config.tcl

rename core_parameter {}

ipx::create_xgui_files $core
ipx::update_checksums $core
ipx::save_core $core

close_project
