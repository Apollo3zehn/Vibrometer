
set partName [lindex $argv 0]
set coreName [lindex $argv 1]
set sourceDirectoryPath [lindex $argv 2]
set targetDirectoryPath [lindex $argv 3]

set elementSet [split $coreName _]
set projectName [join [lrange $elementSet 0 end-2] _]
set version [string trimleft [join [lrange $elementSet end-1 end] .] v]

file delete -force $targetDirectoryPath/$coreName $targetDirectoryPath/$projectName.cache $targetDirectoryPath/$projectName.hw $targetDirectoryPath/$projectName.xpr

create_project -part $partName $projectName $targetDirectoryPath
add_files -norecurse [glob $sourceDirectoryPath/$coreName/*.v]

ipx::package_project -import_files -root_dir $targetDirectoryPath/$coreName

set core [ipx::current_core]

set_property VERSION $version $core
set_property NAME $projectName $core
set_property LIBRARY {user} $core
set_property VENDOR {pavel-demin} $core
set_property VENDOR_DISPLAY_NAME {Pavel Demin} $core
set_property COMPANY_URL {https://github.com/pavel-demin/red-pitaya-notes} $core
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
