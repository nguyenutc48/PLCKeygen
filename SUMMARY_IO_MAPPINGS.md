# PLC I/O Mappings - Implementation Summary

## What Was Created

I've generated comprehensive C# code for your PLC I/O mappings covering 3 ports with sensors, switches, and status indicators.

### Files Created

1. **PLCAddresses.IO.cs** (Main definitions)
   - Complete I/O address mappings for Ports 1, 2, and 3
   - MR21/MR22 (Port 1), MR1/MR2 (Port 2), MR31/MR32 (Port 3)
   - IOHelper class for dynamic port access

2. **PLCAddresses.IO.UsageExample.cs** (Detailed examples)
   - 10 comprehensive usage examples
   - Safety monitoring, quality checks, automated sequences
   - Production cycle examples

3. **PLCAddresses.IO.QuickReference.cs** (Quick start)
   - 10 ready-to-use code snippets
   - Copy-paste examples for common tasks
   - Commented address reference

4. **PLCAddresses.IO.README.md** (Full documentation)
   - Complete address mapping tables
   - Integration guide
   - Best practices and troubleshooting

## Quick Start

### 1. Reading a Sensor

```csharp
// Read input sensor on Port 1
bool sensor = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_VIn1);
Console.WriteLine($"Input Sensor: {sensor}");
```

### 2. Checking Emergency Stop

```csharp
bool ems1 = PLCKey.ReadBit(PLCAddresses.Port1_IO.SW_EMS1);
bool ems2 = PLCKey.ReadBit(PLCAddresses.Port2_IO.SW_EMS2);
bool ems3 = PLCKey.ReadBit(PLCAddresses.Port3_IO.SW_EMS3);

if (ems1 || ems2 || ems3)
{
    MessageBox.Show("EMERGENCY STOP ACTIVATED!");
}
```

### 3. Controlling Tower Lights

```csharp
// Green light = success
PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Gre1);

// Red light = error
PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Red1);

// Yellow light = warning
PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Yel1);
```

### 4. Dynamic Port Access

```csharp
// Access any port dynamically
for (int port = 1; port <= 3; port++)
{
    string vinAddr = PLCAddresses.IOHelper.GetSensorInput(port, "VIN");
    bool partPresent = PLCKey.ReadBit(vinAddr);
    Console.WriteLine($"Port {port}: {partPresent}");
}
```

## Address Mapping Summary

### Port 1 (MR21/MR22)
- **Sensors**: Input, Output, Vacuum, Fixture, Door, Quality
- **Switches**: EMS, Start, Stop, Reset, Init
- **Status**: Tower lights (Green, Yellow, Red)

### Port 2 (MR1/MR2)
- Same as Port 1
- **Special**: Includes tester status integration

### Port 3 (MR31/MR32)
- Same as Port 1

## Integration with Your Form1.cs

### Option 1: Add to Timer (Recommended)

```csharp
private void timer1_Tick(object sender, EventArgs e)
{
    // Your existing code...
    txtXCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_X_Master) / 100.0f).ToString();

    // ADD THIS: Monitor I/O status
    UpdateIOStatus();
}

private void UpdateIOStatus()
{
    // Port 1 sensors
    bool p1_input = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_VIn1);
    bool p1_jig = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Jig_OK);

    // Update your UI
    lblPort1Input.Text = p1_input ? "Part Present" : "No Part";
    lblPort1Quality.Text = p1_jig ? "OK" : "NG";
    lblPort1Quality.BackColor = p1_jig ? Color.Green : Color.Red;

    // Check emergency stops
    bool anyEMS = PLCKey.ReadBit(PLCAddresses.Port1_IO.SW_EMS1) ||
                  PLCKey.ReadBit(PLCAddresses.Port2_IO.SW_EMS2) ||
                  PLCKey.ReadBit(PLCAddresses.Port3_IO.SW_EMS3);

    if (anyEMS)
    {
        lblSafety.BackColor = Color.Red;
        lblSafety.Text = "EMERGENCY STOP!";
    }
}
```

### Option 2: Add Button Event Handlers

```csharp
// Add button to check Port 1 status
private void btnCheckPort1_Click(object sender, EventArgs e)
{
    bool partPresent = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_VIn1);
    bool fixtureClosed = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Fix1_Close);
    bool jigOK = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_Jig_OK);

    string status = $"Port 1 Status:\n" +
                   $"Part Present: {partPresent}\n" +
                   $"Fixture: {(fixtureClosed ? "Closed" : "Open")}\n" +
                   $"Quality: {(jigOK ? "OK" : "NG")}";

    MessageBox.Show(status);
}

// Add button to set tower light
private void btnGreenLight1_Click(object sender, EventArgs e)
{
    // Turn off all lights
    PLCKey.ResetBit(PLCAddresses.Port1_IO.Stt_lca_Gre1);
    PLCKey.ResetBit(PLCAddresses.Port1_IO.Stt_lca_Yel1);
    PLCKey.ResetBit(PLCAddresses.Port1_IO.Stt_lca_Red1);

    // Turn on green
    PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Gre1);
}
```

## Common Use Cases

### 1. Safety Monitoring
```csharp
public bool CheckSafety(int port)
{
    string emsAddr = PLCAddresses.IOHelper.GetSensorInput(port, "EMS");
    string doorAddr = PLCAddresses.IOHelper.GetSensorInput(port, "DOOR");

    bool emsOK = !PLCKey.ReadBit(emsAddr);
    bool doorClosed = PLCKey.ReadBit(doorAddr);

    return emsOK && doorClosed;
}
```

### 2. Quality Inspection
```csharp
public string CheckQuality(int port)
{
    string jigOKAddr = PLCAddresses.IOHelper.GetSensorInput(port, "JIG_OK");
    string jigNGAddr = PLCAddresses.IOHelper.GetSensorInput(port, "JIG_NG");

    bool ok = PLCKey.ReadBit(jigOKAddr);
    bool ng = PLCKey.ReadBit(jigNGAddr);

    return ok ? "PASS" : (ng ? "FAIL" : "UNKNOWN");
}
```

### 3. Production Monitoring
```csharp
public void MonitorProduction()
{
    for (int port = 1; port <= 3; port++)
    {
        string vinAddr = PLCAddresses.IOHelper.GetSensorInput(port, "VIN");
        string votAddr = PLCAddresses.IOHelper.GetSensorInput(port, "VOT");

        bool partIn = PLCKey.ReadBit(vinAddr);
        bool partOut = PLCKey.ReadBit(votAddr);

        if (partIn && !partOut)
        {
            Console.WriteLine($"Port {port}: Processing...");
        }
        else if (!partIn && partOut)
        {
            Console.WriteLine($"Port {port}: Complete, unload part");
        }
    }
}
```

## Complete Address Reference

### Port 1 (MR21/MR22)
| Signal | Address | Constant Name |
|--------|---------|---------------|
| Input Sensor | MR2100 | `PLCAddresses.Port1_IO.Ss_VIn1` |
| Output Sensor | MR2101 | `PLCAddresses.Port1_IO.Ss_VOt1` |
| Fixture Close | MR2105 | `PLCAddresses.Port1_IO.Ss_Fix1_Close` |
| Emergency Stop | MR2202 | `PLCAddresses.Port1_IO.SW_EMS1` |
| Start Button | MR2203 | `PLCAddresses.Port1_IO.SW_Start1` |
| Green Light | MR2108 | `PLCAddresses.Port1_IO.Stt_lca_Gre1` |
| Jig OK | MR2112 | `PLCAddresses.Port1_IO.Ss_Jig_OK` |

*See PLCAddresses.IO.README.md for complete tables*

## Testing Your Integration

### Test 1: Read All Sensors
```csharp
private void btnTestSensors_Click(object sender, EventArgs e)
{
    string result = "=== Sensor Status ===\n\n";

    for (int port = 1; port <= 3; port++)
    {
        result += $"Port {port}:\n";
        result += $"  Input: {PLCKey.ReadBit(PLCAddresses.IOHelper.GetSensorInput(port, "VIN"))}\n";
        result += $"  Fixture: {PLCKey.ReadBit(PLCAddresses.IOHelper.GetSensorInput(port, "FIX_CLOSE"))}\n";
        result += $"  Door: {PLCKey.ReadBit(PLCAddresses.IOHelper.GetSensorInput(port, "DOOR"))}\n";
        result += "\n";
    }

    MessageBox.Show(result);
}
```

### Test 2: Cycle Tower Lights
```csharp
private async void btnTestLights_Click(object sender, EventArgs e)
{
    // Test Port 1 lights
    PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Gre1);
    await Task.Delay(1000);
    PLCKey.ResetBit(PLCAddresses.Port1_IO.Stt_lca_Gre1);

    PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Yel1);
    await Task.Delay(1000);
    PLCKey.ResetBit(PLCAddresses.Port1_IO.Stt_lca_Yel1);

    PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Red1);
    await Task.Delay(1000);
    PLCKey.ResetBit(PLCAddresses.Port1_IO.Stt_lca_Red1);

    MessageBox.Show("Light test complete!");
}
```

## Next Steps

1. **Review** the files in your PLCKeygen project
2. **Test** the basic sensor reading examples
3. **Integrate** into Form1.cs using the timer or button handlers
4. **Customize** based on your specific requirements
5. **Refer** to PLCAddresses.IO.README.md for detailed documentation

## Support Files Location

```
PLCKeygen/
├── PLCAddresses.IO.cs                    ← Main definitions
├── PLCAddresses.IO.UsageExample.cs       ← Detailed examples
├── PLCAddresses.IO.QuickReference.cs     ← Copy-paste snippets
└── PLCAddresses.IO.README.md             ← Full documentation
```

## Questions?

- Check **PLCAddresses.IO.README.md** for detailed documentation
- Review **PLCAddresses.IO.QuickReference.cs** for quick examples
- Examine **PLCAddresses.IO.UsageExample.cs** for advanced patterns

All addresses follow the same pattern:
- `PLCAddresses.Port{N}_IO.{SignalName}`
- Dynamic access: `PLCAddresses.IOHelper.GetSensorInput(port, "SENSOR_NAME")`

Happy coding! 🎉
