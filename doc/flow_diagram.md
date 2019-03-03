```mermaid
graph LR
    DA("fa:fa-ellipsis-h ADC (raw)")
    PT(fa:fa-chart-line position tracking)
    FI(fa:fa-filter filter)
    FT(fa:fa-random Fourier transform)
    AS((AXI4-Stream switch))
    RW(fa:fa-memory RAM writer)

    DA --> PT

    subgraph Processing
        PT --> FI
        FI --> FT
    end

    DA --> AS
    PT --> AS
    FI --> AS
    FT --> AS

    AS --> RW
```

```mermaid
graph LR

    laser(laser)
    object[moving object]
    splitterY((splitter))
    splitterY1((splitter))
    splitterY2((splitter))
    isolator(isolator)
    circulator(circulator)
    delay(delay)
    APD1(APD 1)
    APD2(APD 2)

    laser --> isolator 
    isolator --> splitterY
    splitterY --> |object beam| circulator
    circulator --> object
    object --> circulator

    circulator --> splitterY1
    splitterY --> |reference beam| splitterY1
    splitterY1 --> APD1

    circulator --> splitterY2
    splitterY --> |reference beam| delay
    delay --> splitterY2
    splitterY2 --> APD2
```

```mermaid
graph LR
    VI("fa:fa-warehouse<br/>Infrastructur.dll")
    VA(far:fa-handshake<br/>API.dll)
    VT(fa:fa-flask<br/>Testing.dll)
    VS(fa:fa-server<br/>WebServer.dll)
    VC(fa:fa-desktop<br/>WebClient.dll)

    VI --> VA
    VI --> VC

    VA --> VT

    VA --> VS
    VC --> VS
```

```mermaid
graph TD
    VibrometerApi --> AxisSwitch(AxisSwitch)
    VibrometerApi --> SignalGenerator(SignalGenerator)
    VibrometerApi --> DataAcquisition(DataAcquisition)
    VibrometerApi --> PositionTracker(PositionTracker)
    VibrometerApi --> Filter(...)

    AxisSwitch --> Source["ApiSource Source"]

    SignalGenerator --> FmEnabled["bool FmEnabled"]
    FmEnabled--> ShiftCarrier["uint32 ShiftCarrier"]
    ShiftCarrier--> PhaseSignal[...]

    DataAcquisition --> SwitchEnabled["bool SwitchEnabled"]

    PositionTracker--> LogScale["uint32 LogScale"]
    LogScale--> LogCountExtremum["uint32 LogCountExtremum"]
    LogCountExtremum--> ShiftExtremum[...]
```

```mermaid
graph TD
    VibrometerApi --> AxisSwitch(AxisSwitch)
    VibrometerApi --> SignalGenerator(SignalGenerator)
    VibrometerApi --> DataAcquisition(DataAcquisition)
    VibrometerApi --> PositionTracker(PositionTracker)
    VibrometerApi --> Filter(Filter)
    VibrometerApi --> FourierTransform(FourierTransform)
    VibrometerApi --> RamWriter(RamWriter)

    AxisSwitch --> Source["ApiSource Source"]

    SignalGenerator --> FmEnabled["bool FmEnabled"]
    FmEnabled--> ShiftCarrier["uint32 ShiftCarrier"]
    ShiftCarrier--> PhaseSignal["uint32 PhaseSignal"]
    PhaseSignal--> PhaseCarrier["uint32 PhaseCarrier"]

    DataAcquisition --> SwitchEnabled["bool SwitchEnabled"]

    PositionTracker--> LogScale["uint32 LogScale"]
    LogScale--> LogCountExtremum["uint32 LogCountExtremum"]
    LogCountExtremum--> ShiftExtremum["uint32 ShiftExtremum"]
    ShiftExtremum--> Threshold["uint32 Threshold (RO)"]

    Filter --> FI_Enabled["bool Enabled"]
    FI_Enabled --> FI_LogThrottle["uint32 LogThrottle"]

    FourierTransform--> FT_Enabled["bool Enabled"]
    FT_Enabled --> LogCountAverages["uint32 LogCountAverages"]
    LogCountAverages --> FT_LogThrottle["uint32 LogThrottle"]

    RamWriter --> RW_Enabled["bool Enabled"]
    RW_Enabled --> RequestEnabled["bool RequestEnabled"]
    RequestEnabled --> LogLength["uint32 LogLength"]
    LogLength --> RW_LogThrottle["uint32 LogThrottle"]
    RW_LogThrottle --> Address["uint32 Address (WO)"]
    Address--> ReadBuffer["uint32 ReadBuffer(RO)"]
```

<!-- 
{
  "theme": null,
  "themeCSS": "#subGraph0 { fill:white}  .node > rect, circle { stroke:#00BCD4;} .cluster > rect { stroke:#E61E63;} .edgeLabel { display:block;opacity:1;background:white;border-radius:5px;padding:0px 10px;} .node { fill: azure } path { stroke-width: 1.5px; stroke: #00BCD4} polygon{ stroke-width: 1.5px; stroke: #00BCD4} text { fill:rgba(0, 0, 0, 0.65); font-size: 22px; transform: translateX(0px) translateY(15px) } div { text-align:center }",
  "flowchart": {
    "curve": "line"
  }
}
 -->