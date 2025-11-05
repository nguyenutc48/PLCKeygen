# Model Management System - User Guide

## Tổng Quan

Hệ thống quản lý Model cho phép bạn lưu và tải các bộ teaching points khác nhau cho từng model sản phẩm. Mỗi model lưu trữ tất cả 72 teaching points (4 ports × 18 points).

## Vị Trí Trên Giao Diện

Các control Model Management nằm ở **góc dưới bên phải** của tab Motion:

```
┌─────────────────────────────────────┐
│  Model: [ComboBox ▼] [Add] [Del]   │
└─────────────────────────────────────┘
```

- **Label "Model:"**: Nhãn chỉ dẫn
- **ComboBox**: Danh sách các model đã lưu
- **Button "Add"**: Thêm model mới
- **Button "Del"**: Xóa model đã chọn

## Cách Sử Dụng

### 1. Thêm Model Mới (Save Teaching Points)

**Bước 1:** Teaching các điểm cho tất cả 4 ports
- Chọn Port 1-4 lần lượt
- Chuyển sang Teaching Mode (password: 1234)
- Jog đến các vị trí và Save teaching points

**Bước 2:** Nhấn nút "Add"
- Dialog sẽ hiện ra yêu cầu nhập tên model
- Nhập tên model (VD: "Product_A", "Model_123")
- Nhấn OK

**Bước 3:** Xác nhận
- Hệ thống đọc tất cả teaching points từ PLC
- Lưu vào file JSON
- Model mới xuất hiện trong ComboBox
- Thông báo thành công

### 2. Load Model (Restore Teaching Points)

**Bước 1:** Chọn model từ ComboBox
- Click vào ComboBox
- Chọn model muốn load

**Bước 2:** Xác nhận
- Dialog xác nhận sẽ hiện ra:
  ```
  Bạn có muốn load teaching points từ model 'XXX'?

  Thao tác này sẽ ghi đè các teaching points hiện tại trong PLC.
  ```
- Nhấn **Yes** để tiếp tục

**Bước 3:** Teaching points được ghi vào PLC
- Tất cả 72 teaching points được ghi vào PLC (4 ports)
- Các button Save chuyển sang màu xanh
- Thông báo load thành công

### 3. Xóa Model

**Bước 1:** Chọn model cần xóa từ ComboBox

**Bước 2:** Nhấn nút "Del"

**Bước 3:** Xác nhận xóa
- Dialog xác nhận:
  ```
  Bạn có chắc muốn xóa model 'XXX'?
  ```
- Nhấn **Yes** để xóa

**Bước 4:** Model bị xóa khỏi danh sách

## Cấu Trúc Dữ Liệu

### Thông Tin Lưu Trữ Cho Mỗi Model

Mỗi model lưu trữ:
- **Model Name**: Tên model
- **Created Date**: Ngày tạo
- **Last Modified**: Ngày chỉnh sửa cuối
- **Description**: Mô tả (tùy chọn)
- **72 Teaching Points**:
  - Port 1: 18 points
  - Port 2: 18 points
  - Port 3: 18 points
  - Port 4: 18 points

### 18 Teaching Points Per Port

Mỗi port có 18 teaching points:

#### Tray Input (4 points)
1. XY Start
2. X End
3. Y End
4. Z Position

#### Tray NG1 (4 points)
5. XY Start
6. X End
7. Y End
8. Z Position

#### Tray NG2 (4 points)
9. XY Start
10. X End
11. Y End
12. Z Position

#### Socket (6 points)
13. XY Position
14. Z Load
15. Z Unload
16. Z Ready
17. F Opened
18. F Closed

#### Camera (2 points) - Shared by both ports
19. XY Position
20. Z Position

### Mỗi Teaching Point Có 6 Trục

Mỗi teaching point lưu trữ:
- **X**: Trục X (Int32)
- **Y**: Trục Y (Int32)
- **Z**: Trục Z (Int32)
- **RI**: Rotation Inner (Int32) - hiện tại chưa sử dụng
- **RO**: Rotation Outer (Int32) - hiện tại chưa sử dụng
- **F**: Focus (Int32)

## Lưu Trữ Dữ Liệu

### Vị Trí File

Models được lưu tại:
```
[Application Folder]/TeachingModels/teaching_models.json
```

Ví dụ:
```
D:\3. Program\C#\PLCKeygen\PLCKeygen\bin\Debug\TeachingModels\teaching_models.json
```

### Format JSON

```json
{
  "Models": [
    {
      "ModelName": "Product_A",
      "CreatedDate": "2025-11-06T10:30:00",
      "LastModified": "2025-11-06T10:30:00",
      "Description": "",
      "Port1": {
        "TrayInputXYStart": { "X": 1000, "Y": 2000, "Z": 3000, "RI": 0, "RO": 0, "F": 100 },
        "TrayInputXEnd": { "X": 1500, "Y": 2000, "Z": 3000, "RI": 0, "RO": 0, "F": 100 },
        ...
      },
      "Port2": { ... },
      "Port3": { ... },
      "Port4": { ... }
    },
    {
      "ModelName": "Product_B",
      ...
    }
  ]
}
```

## Quy Trình Làm Việc Khuyến Nghị

### Scenario 1: Sản Xuất Nhiều Model

1. **Setup lần đầu cho Model A**:
   - Teaching tất cả các điểm cho 4 ports
   - Save model: "Product_A"

2. **Setup lần đầu cho Model B**:
   - Thay đổi teaching points
   - Save model: "Product_B"

3. **Chuyển đổi giữa các model**:
   - Chọn "Product_A" từ ComboBox → Load
   - Máy sẵn sàng chạy Product A
   - Chọn "Product_B" từ ComboBox → Load
   - Máy sẵn sàng chạy Product B

### Scenario 2: Backup và Restore

1. **Backup định kỳ**:
   - Save model với tên có ngày tháng: "Backup_2025_11_06"
   - File JSON tự động lưu

2. **Restore khi cần**:
   - Chọn model backup
   - Load vào PLC

### Scenario 3: Share giữa các máy

1. **Export từ máy A**:
   - Copy file `teaching_models.json`
   - Hoặc dùng USB/Network drive

2. **Import vào máy B**:
   - Paste file `teaching_models.json` vào folder `TeachingModels`
   - Khởi động lại application
   - Tất cả models xuất hiện trong ComboBox

## Lưu Ý Quan Trọng

### ⚠️ Warnings

1. **Ghi đè dữ liệu PLC**:
   - Khi load model, tất cả teaching points trong PLC sẽ bị ghi đè
   - Luôn có confirmation dialog để tránh thao tác nhầm

2. **Tên model không được trùng**:
   - Mỗi model phải có tên unique
   - Nếu trùng tên, hệ thống sẽ báo lỗi

3. **Backup dữ liệu**:
   - Nên backup file JSON định kỳ
   - Copy ra ngoài application folder để tránh mất dữ liệu

4. **Data integrity**:
   - Model lưu trữ snapshot của teaching points tại thời điểm Save
   - Nếu sửa teaching points sau khi Save, cần Save lại model

### ✅ Best Practices

1. **Naming Convention**:
   - Dùng tên rõ ràng: "Product_A", "Model_123"
   - Có thể thêm version: "Product_A_v1", "Product_A_v2"
   - Backup: "Backup_YYYY_MM_DD"

2. **Testing**:
   - Sau khi load model, kiểm tra vài teaching points bằng Go
   - Đảm bảo robot di chuyển đúng vị trí

3. **Documentation**:
   - Ghi chú lại tên model tương ứng với sản phẩm
   - Lưu thông tin version control

## Troubleshooting

### Model không load được?
- ✓ Kiểm tra kết nối PLC
- ✓ Kiểm tra file JSON có tồn tại không
- ✓ Xem log/error messages

### ComboBox rỗng?
- ✓ Kiểm tra folder `TeachingModels` có tồn tại không
- ✓ Kiểm tra file `teaching_models.json` có dữ liệu không
- ✓ Thêm model mới bằng nút "Add"

### Teaching points không đúng sau khi load?
- ✓ Kiểm tra đã chọn đúng model chưa
- ✓ Xác nhận model đó có dữ liệu đúng (mở file JSON)
- ✓ Thử Go đến từng điểm để verify

### File JSON bị corrupt?
- ✓ Restore từ backup
- ✓ Tạo file mới bằng cách xóa file cũ
- ✓ Application sẽ tạo file mới rỗng

## Technical Details

### Classes

#### TeachingPoint
```csharp
public class TeachingPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int RI { get; set; }
    public int RO { get; set; }
    public int F { get; set; }
}
```

#### PortTeachingPoints
Chứa 18 TeachingPoint objects cho 1 port

#### TeachingModel
Chứa 4 PortTeachingPoints (Port1-4) + metadata

#### ModelManager
- SaveModel()
- LoadModel()
- DeleteModel()
- GetModelNames()

### PLC Communication

- **Read**: `PLCKey.ReadInt32(address)`
- **Write**: `PLCKey.WriteInt32(address, value)`
- Addresses được lấy từ `PLCAddresses.Data` class

---

## Support

Nếu có vấn đề, kiểm tra:
1. Log messages trong application
2. File JSON format
3. PLC connection status

Version: PLCKeygen 2025.11.2+
