<img src="./doc/icon.png" width="64" height="64" alt="Icon" />

# MSFS Sailing Physics Tool

A custom sail physics simulation for Microsoft Flight Simulator 2020/2024, allowing boats to sail realistically according to wind direction and strength — instead of behaving like motorboats.

![Example Screenshot](./doc/example.png)

---

## 🧭 Intention

Microsoft Flight Simulator features a few drivable boats, but they do not behave like real sailing vessels. They move regardless of wind and lack realistic sail physics. This tool aims to fix that.

**Goal:** Bring authentic sailboat behavior to MSFS using real-time SimConnect data, enabling:

- Wind-aware sailing with upwind and downwind performance
- Dynamic boom and jib simulation
- A learning tool for basic sailing principles in the simulator

---

## 🛠️ Approach / Architecture

The project is a lightweight `.NET` Windows Forms application that connects to MSFS using **SimConnect**.

### Key Components

- **SimConnect Integration**: Retrieves wind, heading, and velocity data  
- **Custom Sail Model**: Calculates effective speed based on relative wind angle and strength  
- **Boom & Jib Logic**: Simulates sail deflection depending on the point of sail and sheet slack  
- **Lift/Drag Physics**: Simulates sail-generated forces based on angle of attack (AoA), camber (bulge), and wind pressure  
- **Keel Interaction**: Simulated implicitly to counter lateral drift and convert sideways lift into forward motion  
- **Live Visualization**: Compass rose with wind direction, boat speed, boom, and jib angles  
- **Track Mode**: Optional logic to apply only when sail physics are enabled and the user isn't manually controlling throttle  

---

## 📈 Sail Physics Overview

This simulation models sails as lifting surfaces, similar to wings. As wind flows over the curved (cambered) sail surface, it creates **lift** at an angle to the apparent wind. The **keel** (or hull resistance) provides lateral stability and redirects this lift into **forward thrust** — except when sailing too far into the wind ("in irons").

### Lift Efficiency Curve

Lift is calculated based on AoA (angle between wind and sail surface), using a non-linear curve:

```csharp
private double ComputeLiftEfficiency(double aoa)
{
    const double aoaUpperLimit = 45.0;
    const double aoaLowerLimit = 2.0;
    const double peakAoA = 15.0;

    double absAoA = Math.Abs(aoa);
    if (absAoA < aoaLowerLimit || absAoA > aoaUpperLimit)
        return 0.0;

    double normalized = (absAoA - aoaLowerLimit) / (aoaUpperLimit - aoaLowerLimit);
    double peakNorm = (peakAoA - aoaLowerLimit) / (aoaUpperLimit - aoaLowerLimit);
    double x = (normalized - peakNorm) / peakNorm;
    double lift = Math.Cos(x * Math.PI / 2);
    return Math.Pow(lift, 2);
}
```

This creates a realistic bell-shaped performance curve peaking at ~15° AoA.

---

## ⛵ Point of Sail Reference

| Relative Wind Angle (°) | Sail Efficiency | Boom Deflection | Jib Deflection    | Point of Sail              |
|-------------------------|------------------|------------------|-------------------|-----------------------------|
| 0–10                    | 0.00             | 0°               | 0°                | In Irons                    |
| 10–35                   | 0.0 → 0.6        | 0° → 10°         | 0° → 15°          | Close Hauled                |
| 35–50                   | 0.6 → 0.85       | 10° → 25°        | 15°               | Close Reach                 |
| 50–75                   | 0.85 → 1.0       | 25° → 45°        | 15°               | Beam Reach                  |
| 75–110                  | 1.0 → 0.85       | 45° → 60°        | 30°               | Broad Reach                 |
| 110–135                 | 0.85 → 0.7       | 60° → 70°        | 30°               | Broad Reach                 |
| 135–160                 | 0.7 → 0.55       | 70° → 85°        | 30°               | Running                     |
| 160+                    | 0.55             | 85°              | -45° (depowered)  | Dead Run / Wing-on-Wing     |

---

## ▶️ Usage

### Requirements

- Microsoft Flight Simulator 2020 or 2024  
- .NET 9 Runtime (or newer)  
- A sail-capable boat in MSFS (e.g., from the community)

### Recommended Boats

- [Yacht and Sailboat Pack (5 boats)](https://store.flightsim.to/product/yatch-and-sailboat-pack-5-boats)  
- [Yacht and Fishing Boat Pack](https://store.flightsim.to/product/yatch-and-fishing-boat-pack)  
- [45ft Lagoon Catamaran](https://store.flightsim.to/product/45ft-lagoon-catamaran)

### Setup

1. Launch MSFS and load into a drivable boat  
2. Start this app — it will automatically connect via SimConnect  
3. Observe wind direction and sail deflection in the UI  
4. Steer the boat and experience realistic speed variations based on wind angle and sail trim  

---

## 🧪 Debugging & Logs

Debug output is written to `debug.log` in the executable folder. You can monitor it live with:

```powershell
Get-Content debug.log -Wait -Tail 20
```

---

## ⚠️ Known Issues & Limitations

- No differentiation between boat types (small, large, historical, catamarans)  
- Keel force is simulated only implicitly — leeway is not currently modeled  
- Sail physics are simplified for gameplay purposes  
- Jib angle can become unrealistic without physical collision detection
- Wind acceleration may be paused during steering (steering on water gets hard with higher sppeds)
- MSFS aircraft fuselage aerodynamics can cause boats to drift into the wind

---

## 🔮 Planned Features

- [ ] **Realistic sail physics**  
      Incorporate vector-based lift/drag forces and apparent wind  
- [ ] **Selectable simulation modes**  
      Toggle between simplified and full physics models  
- [ ] **Manual sail control**  
      reef sails, deploy spinnaker  
- [ ] **Sail dynamics**  
      Simulate camber, luffing, oversheeting, and blanket effects  
- [ ] **UI Enhancements**  
      Interactive controls, live sail curve visualization  
- [ ] **Keel and rudder model**  
      Model lateral resistance and turning forces more accurately  
- [ ] **Spinnaker support**  
      For broad reach / dead run scenarios

---

## 👋 Contributing

If you're into sailing, SimConnect, or MSFS modding, contributions are welcome! Open an issue or a pull request.

Input from real-world sailors, aerodynamicists, or game developers is especially valuable.

---

## 📜 License

MIT License

© 2025 Cedric Pump

Permission is hereby granted, free of charge, to any person obtaining a copy  
of this software and associated documentation files (the “Software”), to deal  
in the Software without restriction, including without limitation the rights  
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell  
copies of the Software, and to permit persons to whom the Software is  
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in  
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR  
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,  
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE  
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER  
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE  
SOFTWARE.
