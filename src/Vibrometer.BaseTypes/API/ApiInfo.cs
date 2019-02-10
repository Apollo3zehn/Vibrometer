using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Vibrometer.BaseTypes.API
{
    public class ApiInfo : ReadOnlyDictionary<ApiParameter, ApiRecord>
    {
        public static ReadOnlyDictionary<ApiParameter, ApiRecord> Instance { get; }

        static ApiInfo()
        {
            ApiInfo.Instance = new ApiInfo();
        }

        public ApiInfo() : base(ApiInfo.GetDictionary())
        { 
            //
        }

        private static IDictionary<ApiParameter, ApiRecord> GetDictionary()
        {
            List<ApiRecord> apiRecordSet;

            apiRecordSet = new List<ApiRecord>()
            {
                // AxisSwitch
                new ApiRecord(
                        ApiParameter.AS_Commit, ApiGroup.AxisSwitch,
                        "Commit", "<no description available>",
                        offset: 0x0000, shift: 0, size: 32
                    ),
                new ApiRecord(
                        ApiParameter.AS_Source, ApiGroup.AxisSwitch,
                        "Source", "Choose the data stream to be written into the RAM.",
                        offset: 0x0040, shift: 0, size: 32 // actually max = 4, but 0 is converted to 0x8000000, so this check would fail
                    ),

                // Signal Generator
                new ApiRecord(
                        ApiParameter.SG_FmEnabled, ApiGroup.SignalGenerator,
                        "Frequency Modulation", "Enables or disables the frequency modulation feature to generate test signals.",
                        offset: 0x00, shift: 0, size: 1
                    ),
                new ApiRecord(
                        ApiParameter.SG_PhaseSignal, ApiGroup.SignalGenerator,
                        "Phase Signal", "Sets the phase incrementation value of the frequency modulated signal.",
                        offset: 0x00, shift: 1, size: 27
                    ),
                new ApiRecord(
                        ApiParameter.SG_PhaseCarrier, ApiGroup.SignalGenerator,
                        "Phase Carrier", "Sets the phase incrementation value of the carrier signals.",
                        offset: 0x08, shift: 0, size: 27
                    ),

                // Data Acquisition
                new ApiRecord(
                        ApiParameter.DA_SwitchEnabled, ApiGroup.DataAcquisition,
                        "Switch", "Enables or disables the interchange of ADC signals A and B.",
                        offset: 0x00, shift: 0, size: 1
                    ),

                // Position Tracker
                new ApiRecord(
                        ApiParameter.PT_LogScale, ApiGroup.PositionTracker,
                        "Log Scale", "Sets the logarithmic scaling factor of the added or subtracted position.",
                        offset: 0x00, shift: 8, size: 5
                ),
                new ApiRecord(
                        ApiParameter.PT_LogCountExtremum, ApiGroup.PositionTracker, 
                        "Log Count Extremum", "Sets the logarithmic maximum number of measurements to determine the extrema.",
                        offset: 0x00, shift: 3, size: 5
                ),
                new ApiRecord(
                        ApiParameter.PT_ShiftExtremum, ApiGroup.PositionTracker, 
                        "Shift Extremum", "Sets number of right shifts to reduce the upper and lower signal limits for the position tracker.",
                        offset: 0x00, shift: 0, size: 3
                ),
                new ApiRecord(
                        ApiParameter.PT_Threshold, ApiGroup.PositionTracker, 
                        "Threshold", "Gets the current position tracker thresholds.",
                        offset: 0x08, shift: 0, size: 32
                ),

                // Filter
                new ApiRecord(
                        ApiParameter.FI_Enabled, ApiGroup.Filter, 
                        "Enabled", "Enables or disable the filter module. A disabled module is a simple pass-through for the data stream with the throttle parameter still applying.",
                        offset: 0x00, shift: 0, size: 1
                ),
                new ApiRecord(
                        ApiParameter.FI_LogThrottle, ApiGroup.Filter,
                        "Log Throttle", "Sets the logarithmic data stream throttle for the filter.",
                        offset: 0x00, shift: 1, size: 5
                ),

                // FourierTransform
                new ApiRecord(
                        ApiParameter.FT_Enabled, ApiGroup.FourierTransform, 
                        "Enabled", "Enables or disables the Fourier Transform. Disabling the Fourier Transform stops the data transfer.",
                        offset: 0x00, shift: 0, size: 1
                ),
                new ApiRecord(
                        ApiParameter.FT_LogCountAverages, ApiGroup.FourierTransform,
                        "Log Count Averages", "Sets the logarithmic number of Fourier transform frame averages.",
                        offset: 0x00, shift: 1, size: 5
                ),
                new ApiRecord(
                        ApiParameter.FT_LogThrottle, ApiGroup.FourierTransform,
                        "Log Throttle", "Sets the logarithmic data stream throttle for the Fourier Transform.",
                        offset: 0x00, shift: 6, size: 5
                ),

                // RAM Writer
                new ApiRecord(
                        ApiParameter.RW_Enabled, ApiGroup.RamWriter, 
                        "Enabled", "Enables or disables the RAM writer. Disabling the RAM writer stops the data transfer.",
                        offset: 0x00, shift: 0, size: 1
                ),
                new ApiRecord(
                        ApiParameter.RW_RequestEnabled, ApiGroup.RamWriter, 
                        "Request", "Enables or disables the RAM writer read buffer request.",
                        offset: 0x00, shift: 1, size: 1
                ),
                new ApiRecord(
                        ApiParameter.RW_LogLength, ApiGroup.RamWriter,
                        "Log Length", "Sets the logarithmic length of the buffer in multiples of uint32.",
                        offset: 0x00, shift: 2, size: 5
                ),
                new ApiRecord(
                        ApiParameter.RW_LogThrottle, ApiGroup.RamWriter,
                        "Log Throttle", "Sets the logarithmic data stream throttle for the RAM writer.",
                        offset: 0x00, shift: 7, size: 5
                ),
                new ApiRecord(
                        ApiParameter.RW_Address, ApiGroup.RamWriter, 
                        "Address", "Sets the base address of the RAM writer.",
                        offset: 0x08, shift: 0, size: 32
                ),
                new ApiRecord(
                        ApiParameter.RW_ReadBuffer, ApiGroup.RamWriter, 
                        "ReadBuffer", "Gets the address of the currently ready buffer.",
                        offset: 0x08, shift: 0, size: 32
                ),
            };

            return apiRecordSet.ToDictionary(x => x.Parameter, x => x);
        }
    }
}
