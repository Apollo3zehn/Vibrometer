```mermaid
graph LR
    DA("fa:fa-ellipsis-h   ADC (raw)")
    PT(fa:fa-chart-line Position Tracker)
    FI(fa:fa-filter Filter)
    FT(fa:fa-random Fourier Transform)
    AS{AXI-4 Stream Switch}
    RW(RAM writer)
    RAM(fa:fa-memory RAM)

    subgraph Processing
        DA --> PT
        PT --> FI
        FI --> FT
    end

    DA --> AS
    PT --> AS
    FI --> AS
    FT --> AS

    AS --> |AXI-4 Stream| RW
    RW --> |AXI-4| RAM
```

<!-- 
{
    "theme": null,
    "themeCSS": "#subGraph0 { fill:white}  .node > rect { stroke:#00BCD4;} .cluster > rect { stroke:#E61E63;} .edgeLabel { display:block;opacity:1;background:white;border-radius:5px;padding:0px 10px;} .node { fill: azure } path { stroke-width: 1.5px; stroke: #00BCD4} polygon{ stroke-width: 1.5px; stroke: #00BCD4} text { fill:rgba(0, 0, 0, 0.65); font-size: 22px; transform: translateX(0px) translateY(15px) }",
    "flowchart": {
        "curve": "line"
    }
}
 -->