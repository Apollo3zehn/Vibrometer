variable location [file dirname [file normalize [info script]]]

set coreNameSet {
	axi_bram_reader_v1_0
	axi_cfg_register_v1_0
	axis_constant_v1_0
	axis_red_pitaya_adc_v2_0
	axis_red_pitaya_dac_v1_0
}

set partName xc7z010clg400-1
set sourceDirectoryPath $location/red-pitaya-notes/cores
set targetDirectoryPath ./../artifacts/cores

file mkdir ./../artifacts/cores

foreach coreName $coreNameSet {
	set argv "$partName $coreName $sourceDirectoryPath $targetDirectoryPath"
	puts "Generating $coreName";
	puts "===========================";
	source make_core.tcl
}