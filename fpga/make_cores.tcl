variable location [file dirname [file normalize [info script]]]

set coreNameSet {
	axis_complex_averager_v1_0
	axis_counter_v1_0
	axis_data_width_adapter_v1_0
	axis_differentiator_v1_0
	axis_extremum_finder_v1_0
	axis_position_tracker_v1_0
	axis_sniffer_v1_0
	axis_throttler_v1_0
	fm_manager_v1_0
	s2mm_ram_writer_v1_0
	signal_switch_v1_0
	sync_manager_v1_0
	axis_constant_v1_0
	axis_red_pitaya_adc_v2_0
	axis_red_pitaya_dac_v1_0
}

set partName xc7z010clg400-1
set targetDirectoryPath ./../artifacts/cores

file mkdir $targetDirectoryPath

foreach coreName $coreNameSet {

	set elementSet [split $coreName _]
	set projectName [join [lrange $elementSet 0 end-2] _]
	set version [string trimleft [join [lrange $elementSet end-1 end] .] v]

	catch {file delete -force {*}[glob -nocomplain -directory $targetDirectoryPath $projectName*]}

	if {[file isdirectory $location/cores/$coreName]} {
		set sourceDirectoryPath $location/cores
		set local true
	} else {
		set sourceDirectoryPath $location/red-pitaya-notes/cores
		set local false
	}

	puts "Generating $coreName";
	puts "===========================";

	if {$local} {
		set argv "$partName $coreName $projectName $version $sourceDirectoryPath $targetDirectoryPath"
		source make_core.tcl
	} else {
		set argv "$partName $coreName $sourceDirectoryPath $targetDirectoryPath"
		source make_core_rpn.tcl
	}
}