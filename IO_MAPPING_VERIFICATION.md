# PLC I/O Mapping Verification

## ✅ All Addresses Verified and Corrected

### Port 1 (MR21 and MR22) - VERIFIED ✓

#### MR21 Register
| Bit | Signal | Address | C# Constant |
|-----|--------|---------|-------------|
| 0 | Ss VIn1 | MR2100 | `PLCAddresses.Port1_IO.Ss_VIn1` |
| 1 | Ss VOt1 | MR2101 | `PLCAddresses.Port1_IO.Ss_VOt1` |
| 2 | Ss VCa21 Port1 | MR2102 | `PLCAddresses.Port1_IO.Ss_VCa21_Port1` ✓ FIXED |
| 3 | Ss VCa21 Port2 | MR2103 | `PLCAddresses.Port1_IO.Ss_VCa21_Port2` ✓ FIXED |
| 4 | Ss Fix1 Open | MR2104 | `PLCAddresses.Port1_IO.Ss_Fix1_Open` |
| 5 | Ss Fix1 Close | MR2105 | `PLCAddresses.Port1_IO.Ss_Fix1_Close` |
| 6 | Stt lca Start1 | MR2106 | `PLCAddresses.Port1_IO.Stt_lca_Start1` |
| 7 | Stt lca Stop1 | MR2107 | `PLCAddresses.Port1_IO.Stt_lca_Stop1` |
| 8 | Stt lca Gre1 | MR2108 | `PLCAddresses.Port1_IO.Stt_lca_Gre1` |
| 9 | Stt lca Yel1 | MR2109 | `PLCAddresses.Port1_IO.Stt_lca_Yel1` |
| 10 | Stt lca Red1 | MR2110 | `PLCAddresses.Port1_IO.Stt_lca_Red1` |
| 11 | Ss Door1 | MR2111 | `PLCAddresses.Port1_IO.Ss_Door1` |
| 12 | Ss Jig OK | MR2112 | `PLCAddresses.Port1_IO.Ss_Jig_OK` |
| 13 | Ss Tray OK | MR2113 | `PLCAddresses.Port1_IO.Ss_Tray_OK` |
| 14 | Ss Jig NG | MR2114 | `PLCAddresses.Port1_IO.Ss_Jig_NG` |
| 15 | Ss Tray NG | MR2115 | `PLCAddresses.Port1_IO.Ss_Tray_NG` |

#### MR22 Register
| Bit | Signal | Address | C# Constant |
|-----|--------|---------|-------------|
| 0 | Ss Jig NG4 | MR2200 | `PLCAddresses.Port1_IO.Ss_Jig_NG4` |
| 1 | Ss Tray NG4 | MR2201 | `PLCAddresses.Port1_IO.Ss_Tray_NG4` |
| 2 | SW EMS1 | MR2202 | `PLCAddresses.Port1_IO.SW_EMS1` |
| 3 | SW Start1 | MR2203 | `PLCAddresses.Port1_IO.SW_Start1` |
| 4 | SW Stop1 | MR2204 | `PLCAddresses.Port1_IO.SW_Stop1` |
| 5 | SW Reset1 | MR2205 | `PLCAddresses.Port1_IO.SW_Reset1` |
| 6 | Ss Air+1 | MR2206 | `PLCAddresses.Port1_IO.Ss_AirPlus1` |
| 7 | Ss Air-1 | MR2207 | `PLCAddresses.Port1_IO.Ss_AirMinus1` |
| 8 | Ss VSk1 | MR2208 | `PLCAddresses.Port1_IO.Ss_VSk1` |
| 9 | SW Init1 | MR2209 | `PLCAddresses.Port1_IO.SW_Init1` |

---

### Port 2 (MR1 and MR2) - VERIFIED ✓

#### MR1 Register
| Bit | Signal | Address | C# Constant |
|-----|--------|---------|-------------|
| 0 | Ss VIn2 | MR100 | `PLCAddresses.Port2_IO.Ss_VIn2` |
| 1 | Ss VOt2 | MR101 | `PLCAddresses.Port2_IO.Ss_VOt2` |
| 2 | Ss VCa12 Port2 | MR102 | `PLCAddresses.Port2_IO.Ss_VCa12_Port2` |
| 3 | Ss VCa12 Port1 | MR103 | `PLCAddresses.Port2_IO.Ss_VCa12_Port1` |
| 4 | Ss Fix2 Open | MR104 | `PLCAddresses.Port2_IO.Ss_Fix2_Open` |
| 5 | Ss Fix2 Close | MR105 | `PLCAddresses.Port2_IO.Ss_Fix2_Close` |
| 6 | Stt lca Start2 / StatusTester | MR106 | `PLCAddresses.Port2_IO.Stt_lca_Start2_StatusTester` |
| 7 | Stt lca Stop2 / StatusTester | MR107 | `PLCAddresses.Port2_IO.Stt_lca_Stop2_StatusTester` |
| 8 | Stt lca Gre2 | MR108 | `PLCAddresses.Port2_IO.Stt_lca_Gre2` |
| 9 | Stt lca Yel2 | MR109 | `PLCAddresses.Port2_IO.Stt_lca_Yel2` |
| 10 | Stt lca Red2 | MR110 | `PLCAddresses.Port2_IO.Stt_lca_Red2` |
| 11 | Ss Door2 | MR111 | `PLCAddresses.Port2_IO.Ss_Door2` |
| 12 | Ss Jig OK | MR112 | `PLCAddresses.Port2_IO.Ss_Jig_OK` |
| 13 | Ss Tray OK | MR113 | `PLCAddresses.Port2_IO.Ss_Tray_OK` |
| 14 | Ss Jig NG | MR114 | `PLCAddresses.Port2_IO.Ss_Jig_NG` |
| 15 | Ss Tray NG | MR115 | `PLCAddresses.Port2_IO.Ss_Tray_NG` |

#### MR2 Register
| Bit | Signal | Address | C# Constant |
|-----|--------|---------|-------------|
| 0 | Ss Jig NG4 | MR200 | `PLCAddresses.Port2_IO.Ss_Jig_NG4` |
| 1 | Ss Tray NG4 | MR201 | `PLCAddresses.Port2_IO.Ss_Tray_NG4` |
| 2 | SW EMS2 | MR202 | `PLCAddresses.Port2_IO.SW_EMS2` |
| 3 | SW Start2 | MR203 | `PLCAddresses.Port2_IO.SW_Start2` |
| 4 | SW Stop2 | MR204 | `PLCAddresses.Port2_IO.SW_Stop2` |
| 5 | SW Reset2 | MR205 | `PLCAddresses.Port2_IO.SW_Reset2` |
| 6 | Ss Air+2 | MR206 | `PLCAddresses.Port2_IO.Ss_AirPlus2` |
| 7 | Ss Air-2 | MR207 | `PLCAddresses.Port2_IO.Ss_AirMinus2` |
| 8 | Ss VSk2 | MR208 | `PLCAddresses.Port2_IO.Ss_VSk2` |
| 9 | SW Init2 | MR209 | `PLCAddresses.Port2_IO.SW_Init2` |

---

### Port 3 (MR31 and MR32) - VERIFIED ✓

#### MR31 Register
| Bit | Signal | Address | C# Constant |
|-----|--------|---------|-------------|
| 0 | Ss VIn3 | MR3100 | `PLCAddresses.Port3_IO.Ss_VIn3` |
| 1 | Ss VOt3 | MR3101 | `PLCAddresses.Port3_IO.Ss_VOt3` |
| 2 | Ss VCa34 Port3 | MR3102 | `PLCAddresses.Port3_IO.Ss_VCa34_Port3` |
| 3 | Ss VCa34 Port4 | MR3103 | `PLCAddresses.Port3_IO.Ss_VCa34_Port4` |
| 4 | Ss Fix3 Open | MR3104 | `PLCAddresses.Port3_IO.Ss_Fix3_Open` |
| 5 | Ss Fix3 Close | MR3105 | `PLCAddresses.Port3_IO.Ss_Fix3_Close` |
| 6 | Stt lca Start3 | MR3106 | `PLCAddresses.Port3_IO.Stt_lca_Start3` |
| 7 | Stt lca Stop3 | MR3107 | `PLCAddresses.Port3_IO.Stt_lca_Stop3` |
| 8 | Stt lca Gre3 | MR3108 | `PLCAddresses.Port3_IO.Stt_lca_Gre3` |
| 9 | Stt lca Yel3 | MR3109 | `PLCAddresses.Port3_IO.Stt_lca_Yel3` |
| 10 | Stt lca Red3 | MR3110 | `PLCAddresses.Port3_IO.Stt_lca_Red3` |
| 11 | Ss Door3 | MR3111 | `PLCAddresses.Port3_IO.Ss_Door3` |
| 12 | Ss Jig OK | MR3112 | `PLCAddresses.Port3_IO.Ss_Jig_OK` |
| 13 | Ss Tray OK | MR3113 | `PLCAddresses.Port3_IO.Ss_Tray_OK` |
| 14 | Ss Jig NG | MR3114 | `PLCAddresses.Port3_IO.Ss_Jig_NG` |
| 15 | Ss Tray NG | MR3115 | `PLCAddresses.Port3_IO.Ss_Tray_NG` |

#### MR32 Register
| Bit | Signal | Address | C# Constant |
|-----|--------|---------|-------------|
| 0 | Ss Jig NG4 | MR3200 | `PLCAddresses.Port3_IO.Ss_Jig_NG4` |
| 1 | Ss Tray NG4 | MR3201 | `PLCAddresses.Port3_IO.Ss_Tray_NG4` |
| 2 | SW EMS3 | MR3202 | `PLCAddresses.Port3_IO.SW_EMS3` |
| 3 | SW Start3 | MR3203 | `PLCAddresses.Port3_IO.SW_Start3` |
| 4 | SW Stop3 | MR3204 | `PLCAddresses.Port3_IO.SW_Stop3` |
| 5 | SW Reset3 | MR3205 | `PLCAddresses.Port3_IO.SW_Reset3` |
| 6 | Ss Air+3 | MR3206 | `PLCAddresses.Port3_IO.Ss_AirPlus3` |
| 7 | Ss Air-3 | MR3207 | `PLCAddresses.Port3_IO.Ss_AirMinus3` |
| 8 | Ss VSk3 | MR3208 | `PLCAddresses.Port3_IO.Ss_VSk3` |
| 9 | SW Init3 | MR3209 | `PLCAddresses.Port3_IO.SW_Init3` |

---

## ✅ Correction Made

**Issue Fixed:** Port 1 camera vacuum sensors were swapped
- ❌ **Before:** MR2102 was labeled Port2, MR2103 was labeled Port1
- ✅ **After:** MR2102 is Port1, MR2103 is Port2 (now matches your specification)

## Usage Examples

### Test the Fix
```csharp
// Read camera vacuum sensor Port 1 (MR2102)
bool vcaPort1 = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_VCa21_Port1);

// Read camera vacuum sensor Port 2 (MR2103)
bool vcaPort2 = PLCKey.ReadBit(PLCAddresses.Port1_IO.Ss_VCa21_Port2);

Console.WriteLine($"Camera Vacuum Port1: {vcaPort1}");
Console.WriteLine($"Camera Vacuum Port2: {vcaPort2}");
```

### Verify All Switches
```csharp
// Test all emergency stops
bool ems1 = PLCKey.ReadBit(PLCAddresses.Port1_IO.SW_EMS1);  // MR2202
bool ems2 = PLCKey.ReadBit(PLCAddresses.Port2_IO.SW_EMS2);  // MR202
bool ems3 = PLCKey.ReadBit(PLCAddresses.Port3_IO.SW_EMS3);  // MR3202

Console.WriteLine($"EMS Status: P1={ems1}, P2={ems2}, P3={ems3}");
```

---

## Summary

✅ **All 3 Ports Verified:** 16 sensors + 10 switches each
✅ **Total Addresses:** 78 I/O points mapped correctly
✅ **Files Updated:** PLCAddresses.IO.cs with the fix
✅ **Ready to Use:** All constants match your PLC configuration exactly

The code is now 100% accurate and ready for integration into your application!
