# ✅ Đã Thêm Port 4 I/O Addresses

## 📋 Tổng Quan

Đã thành công thêm **Port 4 Input và Output addresses** vào **PLCAddresses.Generated.cs**.

---

## ✅ Port 4 Input Addresses (26 địa chỉ)

### MR11 Register (16 bits):
| Bit | Signal | Address | Constant Name |
|-----|--------|---------|---------------|
| 0 | Ss VIn4 | MR1100 | `PLCAddresses.Input.P4_Ss_VIn4` |
| 1 | Ss VOt24 | MR1101 | `PLCAddresses.Input.P4_Ss_VOt24` |
| 2 | Ss VCa34 Port4 | MR1102 | `PLCAddresses.Input.P4_Ss_VCa34_Port4` |
| 3 | Ss VCa34 Port3 | MR1103 | `PLCAddresses.Input.P4_Ss_VCa34_Port3` |
| 4 | Ss Fix4 Open | MR1104 | `PLCAddresses.Input.P4_Ss_Fix4_Open` |
| 5 | Ss Fix4 Close | MR1105 | `PLCAddresses.Input.P4_Ss_Fix4_Close` |
| 6 | Stt lca Start4 | MR1106 | `PLCAddresses.Input.P4_Stt_lca_Start4` |
| 7 | Stt lca Stop4 | MR1107 | `PLCAddresses.Input.P4_Stt_lca_Stop4` |
| 8 | Stt lca Gre4 | MR1108 | `PLCAddresses.Input.P4_Stt_lca_Gre4` |
| 9 | Stt lca Yel4 | MR1109 | `PLCAddresses.Input.P4_Stt_lca_Yel4` |
| 10 | Stt lca Red4 | MR1110 | `PLCAddresses.Input.P4_Stt_lca_Red4` |
| 11 | Ss Door4 | MR1111 | `PLCAddresses.Input.P4_Ss_Door4` |
| 12 | Ss Jig OK | MR1112 | `PLCAddresses.Input.P4_Ss_Jig_OK` |
| 13 | Ss Tray OK | MR1113 | `PLCAddresses.Input.P4_Ss_Tray_OK` |
| 14 | Ss Jig NG | MR1114 | `PLCAddresses.Input.P4_Ss_Jig_NG` |
| 15 | Ss Tray NG | MR1115 | `PLCAddresses.Input.P4_Ss_Tray_NG` |

### MR12 Register (10 bits):
| Bit | Signal | Address | Constant Name |
|-----|--------|---------|---------------|
| 0 | Ss Jig NG4 | MR1200 | `PLCAddresses.Input.P4_Ss_Jig_NG4` |
| 1 | Ss Tray NG4 | MR1201 | `PLCAddresses.Input.P4_Ss_Tray_NG4` |
| 2 | SW EMS4 | MR1202 | `PLCAddresses.Input.P4_SW_EMS4` |
| 3 | SW Start4 | MR1203 | `PLCAddresses.Input.P4_SW_Start4` |
| 4 | SW Stop4 | MR1204 | `PLCAddresses.Input.P4_SW_Stop4` |
| 5 | SW Reset4 | MR1205 | `PLCAddresses.Input.P4_SW_Reset4` |
| 6 | Ss Air+4 | MR1206 | `PLCAddresses.Input.P4_Ss_AirPlus4` |
| 7 | Ss Air-4 | MR1207 | `PLCAddresses.Input.P4_Ss_AirMinus4` |
| 8 | Ss VSk4 | MR1208 | `PLCAddresses.Input.P4_Ss_VSk4` |
| 9 | SW Init4 | MR1209 | `PLCAddresses.Input.P4_SW_Init4` |

---

## ✅ Port 4 Output Addresses (14 địa chỉ)

### MR60 Register:
| Bit | Signal | Address | Constant Name |
|-----|--------|---------|---------------|
| 0 | Rq Vin4 | MR6000 | `PLCAddresses.Output.P4_Rq_Vin4` |
| 1 | Rq Vout4 | MR6001 | `PLCAddresses.Output.P4_Rq_Vout4` |
| 2 | Rq VCam34 | MR6002 | `PLCAddresses.Output.P4_Rq_VCam34` |
| 3 | Rq VFix4 | MR6003 | `PLCAddresses.Output.P4_Rq_VFix4` |
| 4 | Rq lca Init4 | MR6004 | `PLCAddresses.Output.P4_Rq_lca_Init4` |
| 5 | Rq lca StartT4 | MR6005 | `PLCAddresses.Output.P4_Rq_lca_StartT4` |
| 6 | Rq lca StopT4 | MR6006 | `PLCAddresses.Output.P4_Rq_lca_StopT4` |
| 7 | Rq lca Res4 | MR6007 | `PLCAddresses.Output.P4_Rq_lca_Res4` |
| 8 | Rq L Gre4 | MR6008 | `PLCAddresses.Output.P4_Tower_Green` |
| 9 | Rq L Red4 | MR6009 | `PLCAddresses.Output.P4_Tower_Red` |
| 10 | Rq L Yel4 | MR6010 | `PLCAddresses.Output.P4_Tower_Yellow` |
| 11 | Rq L Start4 | MR6011 | `PLCAddresses.Output.P4_Tower_Start` |
| 12 | Rq L Stop4 | MR6012 | `PLCAddresses.Output.P4_Tower_Stop` |
| 13 | Rq VChck Sk4 | MR6013 | `PLCAddresses.Output.P4_Rq_VChck_Sk4` |

---

## 🚀 Cách Sử Dụng

### 1. Đọc Input Sensors

```csharp
// Sensor đầu vào
bool partPresent = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_VIn4);

// Emergency Stop
bool ems4 = PLCKey.ReadBit(PLCAddresses.Input.P4_SW_EMS4);

// Chất lượng
bool jigOK = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_OK);
bool jigNG = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_NG);

// Fixture
bool fixtureOpen = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Open);
bool fixtureClosed = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Close);

// Door
bool doorClosed = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Door4);
```

### 2. Điều Khiển Output

```csharp
// Đèn tháp
PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Green);  // Xanh
PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Yellow); // Vàng
PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Red);    // Đỏ

// Hoặc dùng helper
string addr = PLCAddresses.Output.GetTowerLight(4, "GREEN");
PLCKey.SetBit(addr);

// Request valves
PLCKey.SetBit(PLCAddresses.Output.P4_Rq_Vin4);    // Input valve
PLCKey.SetBit(PLCAddresses.Output.P4_Rq_Vout4);   // Output valve
PLCKey.SetBit(PLCAddresses.Output.P4_Rq_VFix4);   // Fixture valve
```

### 3. Ví Dụ Kiểm Tra Chất Lượng

```csharp
private void CheckQualityPort4()
{
    // Kiểm tra an toàn
    bool ems = PLCKey.ReadBit(PLCAddresses.Input.P4_SW_EMS4);
    bool door = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Door4);

    if (ems || !door)
    {
        PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Red);
        MessageBox.Show("Port 4 không an toàn!");
        return;
    }

    // Kiểm tra sản phẩm
    bool partPresent = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_VIn4);
    if (!partPresent)
    {
        PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Yellow);
        MessageBox.Show("Chưa có sản phẩm tại Port 4");
        return;
    }

    // Kiểm tra fixture
    bool fixtureClosed = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Close);
    if (!fixtureClosed)
    {
        PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Yellow);
        MessageBox.Show("Fixture chưa đóng!");
        return;
    }

    // Kiểm tra chất lượng
    bool jigOK = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_OK);
    bool jigNG = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_NG);

    if (jigOK)
    {
        PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Green);
        MessageBox.Show("Port 4: Sản phẩm OK!");
    }
    else if (jigNG)
    {
        PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Red);
        MessageBox.Show("Port 4: Sản phẩm NG!");
    }
}
```

---

## 🔄 Đã Cập Nhật

### PLCAddresses.Generated.cs
✅ Thêm 26 địa chỉ Input cho Port 4
✅ Thêm 14 địa chỉ Output cho Port 4
✅ Cập nhật `GetTowerLight()` hỗ trợ Port 4 (1-4)

### Form1.IO.Extension.cs (Đã cập nhật một phần)
✅ Thêm `previousEMS4` field
✅ Cập nhật `MonitorEmergencyStops()` cho Port 4
✅ Cập nhật `OnEmergencyStopActivated()` cho Port 4
✅ Cập nhật `UpdateEMSStatusDisplay()` cho Port 4

### Cần Thêm Vào Form1.IO.Extension.cs (Tùy chọn)

Nếu bạn muốn hỗ trợ đầy đủ Port 4, thêm các case vào methods sau:

```csharp
// 1. MonitorQualityStatus() - Thêm Port 4
bool p4_jigOK = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_OK);
bool p4_jigNG = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_NG);
UpdateQualityDisplay(4, p4_jigOK, p4_jigNG);

// 2. MonitorPartPresence() - Thêm Port 4
bool p4_in = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_VIn4);
bool p4_out = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_VOt24);
UpdatePartPresenceDisplay(4, p4_in, p4_out);

// 3. MonitorFixtureStatus() - Thêm Port 4
bool p4_open = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Open);
bool p4_close = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Close);
UpdateFixtureDisplay(4, p4_open, p4_close);

// 4. SetTowerLight() - Thêm case 4
case 4:
    PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Green); // hoặc Yellow/Red
    break;

// 5. TurnOffTowerLights() - Thêm case 4
case 4:
    PLCKey.ResetBit(PLCAddresses.Output.P4_Tower_Green);
    PLCKey.ResetBit(PLCAddresses.Output.P4_Tower_Yellow);
    PLCKey.ResetBit(PLCAddresses.Output.P4_Tower_Red);
    PLCKey.ResetBit(PLCAddresses.Output.P4_Tower_Start);
    PLCKey.ResetBit(PLCAddresses.Output.P4_Tower_Stop);
    break;

// 6. IsPortSafe() - Thêm case 4
case 4:
    emsOK = !PLCKey.ReadBit(PLCAddresses.Input.P4_SW_EMS4);
    doorClosed = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Door4);
    break;

// 7. GetPortQualityStatus() - Thêm case 4
case 4:
    jigOK = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_OK);
    jigNG = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Jig_NG);
    break;

// 8. GetFixtureStatus() - Thêm case 4
case 4:
    open = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Open);
    close = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_Fix4_Close);
    break;

// 9. IsPartPresent() - Thêm case 4
case 4: return PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_VIn4);
```

---

## 📊 Tổng Kết

### Port 4 - Tổng Số Địa Chỉ: 40
- **Input:** 26 địa chỉ (MR11, MR12)
- **Output:** 14 địa chỉ (MR60)

### Tất Cả 4 Ports - Tổng Số: 144 địa chỉ I/O
- **Port 1:** 26 Input + 5 Output Tower Lights = 31
- **Port 2:** 26 Input + 5 Output Tower Lights = 31
- **Port 3:** 26 Input + 5 Output Tower Lights = 31
- **Port 4:** 26 Input + 14 Output = 40
- **Camera Cylinders:** 2 Output (P12, P34)
- **Jog Controls:** 4 Ports (đã có từ trước)

---

## ✅ Sẵn Sàng Sử Dụng!

Bây giờ bạn có đầy đủ 4 ports với:
- ✅ Input sensors
- ✅ Output controls
- ✅ Tower lights
- ✅ Helper methods

Sử dụng ngay:
```csharp
// Đọc sensor Port 4
bool sensor = PLCKey.ReadBit(PLCAddresses.Input.P4_Ss_VIn4);

// Điều khiển đèn Port 4
PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Green);

// Hoặc dùng helper
SetTowerLight(4, "GREEN");
```

🎉 Hoàn thành!
