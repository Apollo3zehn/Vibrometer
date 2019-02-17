```mermaid
graph LR
    DA("fa:fa-ellipsis-h   ADC (raw)")
    PT(fa:fa-chart-line position tracking)
    FI(fa:fa-filter filter)
    FT(fa:fa-random Fourier transform)
    AS((AXI-4 stream switch))
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
    splitterX((splitter))
    splitterY((splitter))
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
    splitterY --> |reference beam| delay
    delay --> splitterX
    circulator --> splitterX
    splitterX --> APD1
    splitterX --> APD2
```

<!-- 
{
    "theme": null,
    "themeCSS": "#subGraph0 { fill:white}  .node > rect, circle { stroke:#00BCD4;} .cluster > rect { stroke:#E61E63;} .edgeLabel { display:block;opacity:1;background:white;border-radius:5px;padding:0px 10px;} .node { fill: azure } path { stroke-width: 1.5px; stroke: #00BCD4} polygon{ stroke-width: 1.5px; stroke: #00BCD4} text { fill:rgba(0, 0, 0, 0.65); font-size: 22px; transform: translateX(0px) translateY(15px) }",
    "flowchart": {
        "curve": "line"
    }
}
 -->