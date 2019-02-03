using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Vibrometer.Shared.API
{
    public class ApiInfo : ReadOnlyDictionary<ApiMethod, ApiRecord>
    {
        public ApiInfo() : base(ApiInfo.GetDictionary())
        { 
            //
        }

        private static IDictionary<ApiMethod, ApiRecord> GetDictionary()
        {
            List<ApiRecord> apiRecordSet;

            apiRecordSet = new List<ApiRecord>()
            {
                // General
                new ApiRecord(
                        ApiMethod.GE_Source, ApiGroup.General, "Source",
                        offset: 0x0000, shift: 0, size: 32 // actually max = 4, but 0 is converted to 0x8000000, so this check would fail
                    ),

                // Signal Generator
                new ApiRecord(
                        ApiMethod.SG_FmEnabled, ApiGroup.SignalGenerator, "Frequency Modulation Enabled",
                        offset: 0x00, shift: 0, size: 1
                    ),

                new ApiRecord(
                        ApiMethod.SG_PhaseSignal, ApiGroup.SignalGenerator, "Phase Signal",
                        offset: 0x00, shift: 1, size: 27
                    ),

                new ApiRecord(
                        ApiMethod.SG_PhaseCarrier, ApiGroup.SignalGenerator, "Phase Carrier",
                        offset: 0x08, shift: 0, size: 27
                    ),

                // Data Acquisition
                new ApiRecord(
                        ApiMethod.DA_SwitchEnabled, ApiGroup.DataAcquisition, "Switch Enabled",
                        offset: 0x00, shift: 0, size: 1
                    ),

                // Position Tracker
                new ApiRecord(
                        ApiMethod.PT_LogScale, ApiGroup.PositionTracker, "Log Scale",
                        offset: 0x00, shift: 8, size: 5
                ),

                new ApiRecord(
                        ApiMethod.PT_LogCountExtremum, ApiGroup.PositionTracker, "Log Count Extremum",
                        offset: 0x00, shift: 3, size: 5
                ),
                new ApiRecord(
                        ApiMethod.PT_ShiftExtremum, ApiGroup.PositionTracker, "Shift Extremum",
                        offset: 0x00, shift: 0, size: 3
                ),
                new ApiRecord(
                        ApiMethod.PT_Threshold, ApiGroup.PositionTracker, "Threshold",
                        offset: 0x08, shift: 0, size: 32
                ),

                // Filter
                new ApiRecord(
                        ApiMethod.FI_LogThrottle, ApiGroup.Filter, "Log Throttle",
                        offset: 0x00, shift: 0, size: 5
                ),

                // FourierTransform
                new ApiRecord(
                        ApiMethod.FT_Enabled, ApiGroup.FourierTransform, "Enabled",
                        offset: 0x00, shift: 0, size: 1
                ),
                new ApiRecord(
                        ApiMethod.FT_LogCountAverages, ApiGroup.FourierTransform, "Log Count Averages",
                        offset: 0x00, shift: 1, size: 5
                ),
                new ApiRecord(
                        ApiMethod.FT_LogThrottle, ApiGroup.FourierTransform, "Log Throttle",
                        offset: 0x00, shift: 6, size: 5
                ),

                // RAM Writer
                new ApiRecord(
                        ApiMethod.RW_Enabled, ApiGroup.RamWriter, "Enabled",
                        offset: 0x00, shift: 0, size: 1
                ),
                new ApiRecord(
                        ApiMethod.RW_RequestEnabled, ApiGroup.RamWriter, "Request Enabled",
                        offset: 0x00, shift: 1, size: 1
                ),
                new ApiRecord(
                        ApiMethod.RW_LogLength, ApiGroup.RamWriter, "Log Length",
                        offset: 0x00, shift: 2, size: 5
                ),
                new ApiRecord(
                        ApiMethod.RW_LogThrottle, ApiGroup.RamWriter, "Log Throttle",
                        offset: 0x00, shift: 7, size: 5
                ),
                new ApiRecord(
                        ApiMethod.RW_Address, ApiGroup.RamWriter, "Address",
                        offset: 0x08, shift: 0, size: 32
                ),
                new ApiRecord(
                        ApiMethod.RW_ReadBuffer, ApiGroup.RamWriter, "ReadBuffer",
                        offset: 0x08, shift: 0, size: 32
                ),
            };

            return apiRecordSet.ToDictionary(x => x.Method, x => x);
        }
    }
}
