# Step-by-Step Integration Guide

## How to Add I/O Monitoring to Your Form1.cs

### Method 1: Quick Integration (Recommended)

#### Step 1: Add the Extension File to Your Project

The file `Form1.IO.Extension.cs` is already created as a **partial class**. It will automatically merge with your existing Form1.cs.

Just make sure it's included in your project - Visual Studio should detect it automatically.

#### Step 2: Add One Line to timer1_Tick

Open **Form1.cs** and find the `timer1_Tick` method (around line 100).

**ADD THIS ONE LINE:**

```csharp
private void timer1_Tick(object sender, EventArgs e)
{
    txtXCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_X_Master) / 100.0f).ToString();
    txtYCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_Y_Master) / 100.0f).ToString();
    txtRCurMasPort1.Text = (PLCKey.ReadInt32(PLCAddresses.Data.P1_R_Master) / 10.0f).ToString();
    // ... your existing code ...

    // ADD THIS LINE ⬇️
    UpdateIOStatus();  // ← Monitor I/O sensors
}
```

That's it! Now you have automatic I/O monitoring.

---

### Method 2: Manual Integration (More Control)

If you prefer to manually add the methods to your existing Form1.cs:

#### Step 1: Add Field Variables

Add these fields to your Form1 class (around line 17):

```csharp
public partial class Form1 : Form
{
    PLCKeygen.PLCKeyence PLCKey;
    private CameraTcpClient cameraClient12;
    private CameraTcpClient cameraClient34;
    private bool stopHandEye1 = false;
    private bool stopHandEye2 = false;

    // ADD THESE FIELDS ⬇️
    private bool previousEMS1 = false;
    private bool previousEMS2 = false;
    private bool previousEMS3 = false;
```

#### Step 2: Add UpdateIOStatus Method

Copy this method into your Form1 class:

```csharp
private void UpdateIOStatus()
{
    try
    {
        // Monitor emergency stops
        bool ems1 = PLCKey.ReadBit(PLCAddresses.Port1_IO.SW_EMS1);
        bool ems2 = PLCKey.ReadBit(PLCAddresses.Port2_IO.SW_EMS2);
        bool ems3 = PLCKey.ReadBit(PLCAddresses.Port3_IO.SW_EMS3);

        // Alert if EMS was just pressed
        if ((ems1 && !previousEMS1) || (ems2 && !previousEMS2) || (ems3 && !previousEMS3))
        {
            string msg = "EMERGENCY STOP!\n";
            if (ems1) msg += "Port 1\n";
            if (ems2) msg += "Port 2\n";
            if (ems3) msg += "Port 3\n";
            MessageBox.Show(msg, "EMERGENCY", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        previousEMS1 = ems1;
        previousEMS2 = ems2;
        previousEMS3 = ems3;
    }
    catch { }
}
```

#### Step 3: Call from Timer

Add to timer1_Tick:

```csharp
private void timer1_Tick(object sender, EventArgs e)
{
    // ... existing code ...

    UpdateIOStatus();  // ← Add this
}
```

---

## Usage Examples

### Example 1: Check Safety Before Operation

```csharp
private void btnStartProcess_Click(object sender, EventArgs e)
{
    // Check if Port 1 is safe
    if (!IsPortSafe(1))
    {
        MessageBox.Show("Port 1 not safe! Check EMS and door.", "Safety Error");
        return;
    }

    // Safe to proceed
    SetTowerLight(1, "GREEN");
    // ... start your process ...
}
```

### Example 2: Monitor Quality and Set Lights

```csharp
private void btnCheckQuality1_Click(object sender, EventArgs e)
{
    string quality = GetPortQualityStatus(1);

    if (quality == "OK")
    {
        SetTowerLight(1, "GREEN");
        MessageBox.Show("Part OK!", "Quality Check");
    }
    else if (quality == "NG")
    {
        SetTowerLight(1, "RED");
        MessageBox.Show("Part NG - Reject", "Quality Check");
    }
    else
    {
        SetTowerLight(1, "YELLOW");
        MessageBox.Show("No part detected", "Quality Check");
    }
}
```

### Example 3: Wait for Part

```csharp
private async void btnWaitForPart_Click(object sender, EventArgs e)
{
    SetTowerLight(1, "YELLOW");  // Waiting

    DateTime start = DateTime.Now;
    while ((DateTime.Now - start).TotalSeconds < 10)
    {
        if (IsPartPresent(1))
        {
            SetTowerLight(1, "GREEN");
            MessageBox.Show("Part detected!");
            return;
        }
        await Task.Delay(100);
    }

    SetTowerLight(1, "RED");
    MessageBox.Show("Timeout - no part");
}
```

### Example 4: Check Fixture Before Closing

```csharp
private void btnCloseFixture1_Click(object sender, EventArgs e)
{
    string fixtureStatus = GetFixtureStatus(1);

    if (fixtureStatus == "OPEN")
    {
        // Safe to close
        // ... send close command to PLC ...
        MessageBox.Show("Closing fixture...");
    }
    else if (fixtureStatus == "CLOSED")
    {
        MessageBox.Show("Fixture already closed");
    }
    else
    {
        MessageBox.Show("Fixture is moving, please wait");
    }
}
```

---

## Available Helper Methods

After integration, you'll have these methods available:

### Safety
```csharp
bool safe = IsPortSafe(1);  // Check EMS and door
```

### Quality
```csharp
string quality = GetPortQualityStatus(1);  // Returns "OK", "NG", or "UNKNOWN"
```

### Fixture
```csharp
string fixture = GetFixtureStatus(1);  // Returns "OPEN", "CLOSED", or "MOVING"
```

### Part Presence
```csharp
bool haspart = IsPartPresent(1);  // Check if part is at input sensor
```

### Tower Lights
```csharp
SetTowerLight(1, "GREEN");   // Success
SetTowerLight(1, "YELLOW");  // Warning/Waiting
SetTowerLight(1, "RED");     // Error
SetTowerLight(1, "OFF");     // Turn off all
```

---

## Testing Your Integration

### Test 1: Emergency Stop Alert

1. Run your application
2. Press any emergency stop button on the machine
3. You should see an alert message
4. Tower light should turn RED

### Test 2: Quality Check

```csharp
// Add a test button to your form
private void btnTestQuality_Click(object sender, EventArgs e)
{
    MessageBox.Show(
        $"Port 1: {GetPortQualityStatus(1)}\n" +
        $"Port 2: {GetPortQualityStatus(2)}\n" +
        $"Port 3: {GetPortQualityStatus(3)}"
    );
}
```

### Test 3: Tower Light Test

```csharp
// Test all lights sequentially
private async void btnTestLights_Click(object sender, EventArgs e)
{
    for (int port = 1; port <= 3; port++)
    {
        SetTowerLight(port, "GREEN");
        await Task.Delay(1000);

        SetTowerLight(port, "YELLOW");
        await Task.Delay(1000);

        SetTowerLight(port, "RED");
        await Task.Delay(1000);

        SetTowerLight(port, "OFF");
    }

    MessageBox.Show("Light test complete!");
}
```

---

## Troubleshooting

### "UpdateIOStatus not found"
- Make sure Form1.IO.Extension.cs is included in your project
- Check that it's marked as a **partial class**
- Rebuild your solution

### "Emergency stop not triggering"
- Verify PLC addresses are correct (check IO_MAPPING_VERIFICATION.md)
- Test with: `bool ems = PLCKey.ReadBit(PLCAddresses.Port1_IO.SW_EMS1);`
- Check PLC wiring

### "Tower lights not working"
- Verify output addresses in PLC configuration
- Test manually: `PLCKey.SetBit(PLCAddresses.Port1_IO.Stt_lca_Gre1);`
- Check tower light power supply

---

## Next Steps

1. ✅ Add `UpdateIOStatus();` to timer1_Tick
2. ✅ Test emergency stop detection
3. ✅ Add quality monitoring
4. ✅ Implement tower light control
5. ✅ Create safety checks before operations

## Summary

**Minimum integration:** Just add one line to timer1_Tick:
```csharp
UpdateIOStatus();
```

This gives you:
- ✅ Automatic emergency stop monitoring
- ✅ Quality status tracking
- ✅ Part presence detection
- ✅ Fixture status monitoring
- ✅ Helper methods for safety and control

**Files you have:**
- `PLCAddresses.IO.cs` - All address definitions ✅
- `Form1.IO.Extension.cs` - Extension methods ✅
- `PLCAddresses.IO.UsageExample.cs` - More examples
- `PLCAddresses.IO.QuickReference.cs` - Code snippets
- `IO_MAPPING_VERIFICATION.md` - Address verification

You're ready to go! 🚀
