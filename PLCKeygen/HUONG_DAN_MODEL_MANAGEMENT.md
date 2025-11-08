# Hướng Dẫn Sử Dụng Tính Năng Quản Lý Model Teaching

## Tổng Quan

Tính năng quản lý Model cho phép bạn:
- **Lưu** tất cả teaching points (4 ports) vào file JSON với tên model
- **Load** teaching points từ model đã lưu vào PLC
- **Xóa** model không còn sử dụng
- Quản lý nhiều model sản phẩm khác nhau một cách dễ dàng

## Vị Trí Controls

Tất cả controls nằm trong **GroupBox "Model"** ở tab Teaching:
- **ComboBox cbbModel**: Chọn model để load
- **Button "Add"**: Lưu teaching points hiện tại vào model mới
- **Button "Del"**: Xóa model đã chọn

## Quy Tắc Sử Dụng

### ⚠️ QUAN TRỌNG

1. **Buttons Add/Delete chỉ hiển thị khi:**
   - Đang ở chế độ **Teaching Mode** (đã nhập password 1234)
   - Khi chuyển sang Jog Mode hoặc chuyển Port → buttons tự động ẩn

2. **Load model chỉ được phép khi:**
   - Đang ở chế độ **Teaching Mode**
   - Nếu chọn model ở Jog Mode → Hiển thị cảnh báo yêu cầu chuyển sang Teaching Mode

3. **Luồng an toàn:**
   ```
   Jog Mode → Teaching Mode (nhập password) → Buttons hiển thị → Thao tác với model
   ```

## Hướng Dẫn Sử Dụng Chi Tiết

### 1. Thêm Model Mới (Save Teaching Points)

**Bước 1:** Chuyển sang Teaching Mode
- Click radio button **"Teaching"**
- Nhập password: **1234**
- Buttons "Add" và "Del" sẽ hiển thị

**Bước 2:** Teaching các điểm
- Chọn Port muốn teaching
- Di chuyển robot đến các vị trí cần lưu
- Click các nút "Save" tương ứng (ví dụ: Save Point Socket, Save Point Tray Input, v.v.)
- Lặp lại cho tất cả 4 ports

**Bước 3:** Lưu vào Model
- Click button **"Add"**
- Nhập tên model (ví dụ: "Model_ABC_2024")
- Click OK

**Kết quả:**
- Model được lưu vào file: `TeachingModels/teaching_models.json`
- Tên model xuất hiện trong ComboBox
- Thông báo: "Model 'XXX' đã được lưu thành công!"

### 2. Load Model (Restore Teaching Points)

**⚠️ Lưu ý:** Chỉ được phép khi đang ở **Teaching Mode**

**Bước 1:** Đảm bảo đang ở Teaching Mode
- Nếu đang ở Jog Mode → Chuyển sang Teaching Mode trước

**Bước 2:** Chọn model từ ComboBox
- Click vào ComboBox **cbbModel**
- Chọn model cần load

**Bước 3:** Xác nhận
- Hệ thống hiển thị dialog xác nhận:
  ```
  Bạn có muốn load teaching points từ model 'XXX'?

  Thao tác này sẽ ghi đè các teaching points hiện tại trong PLC.
  ```
- Click **Yes** để tiếp tục
- Click **No** để hủy

**Kết quả:**
- Tất cả teaching points (4 ports) được ghi vào PLC
- Màu các button Save chuyển sang xanh (có dữ liệu)
- Thông báo: "Model 'XXX' đã được load thành công!"

### 3. Xóa Model

**⚠️ Lưu ý:** Chỉ được phép khi đang ở **Teaching Mode**

**Bước 1:** Chọn model cần xóa
- Click vào ComboBox **cbbModel**
- Chọn model muốn xóa

**Bước 2:** Click button "Del"
- Hệ thống hiển thị dialog xác nhận:
  ```
  Bạn có chắc muốn xóa model 'XXX'?
  ```
- Click **Yes** để xác nhận xóa
- Click **No** để hủy

**Kết quả:**
- Model bị xóa khỏi file JSON
- Tên model biến mất khỏi ComboBox
- Thông báo: "Model 'XXX' đã được xóa!"

## Luồng Hoạt Động (Workflow)

### Kịch Bản 1: Tạo Model Mới Cho Sản Phẩm ABC

```
1. Chuyển sang Teaching Mode (password: 1234)
   ↓
2. Chọn Port 1
   ↓
3. Teaching tất cả điểm cho Port 1
   (Tray Input, Tray NG1, NG2, Socket, Camera)
   ↓
4. Lặp lại cho Port 2, 3, 4
   ↓
5. Click "Add" → Nhập tên "Model_ABC"
   ↓
6. Model được lưu thành công!
```

### Kịch Bản 2: Chuyển Đổi Giữa Các Model

```
1. Đang sản xuất Model A
   ↓
2. Cần chuyển sang Model B
   ↓
3. Chuyển sang Teaching Mode
   ↓
4. Chọn "Model_B" từ ComboBox
   ↓
5. Xác nhận Yes
   ↓
6. Tất cả teaching points của Model B được load vào PLC
   ↓
7. Chuyển về Jog Mode để tiếp tục sản xuất
```

### Kịch Bản 3: Xóa Model Cũ Không Dùng

```
1. Chuyển sang Teaching Mode
   ↓
2. Chọn model cũ cần xóa
   ↓
3. Click "Del"
   ↓
4. Xác nhận Yes
   ↓
5. Model bị xóa khỏi danh sách
```

## Cảnh Báo & Lỗi Thường Gặp

### ❌ "Vui lòng chuyển sang chế độ Teaching trước khi load model!"
- **Nguyên nhân:** Đang ở Jog Mode mà cố chọn model
- **Giải pháp:** Chuyển sang Teaching Mode (nhập password 1234)

### ❌ "Model 'XXX' đã tồn tại!"
- **Nguyên nhân:** Tên model đã được dùng
- **Giải pháp:** Đặt tên khác hoặc xóa model cũ trước

### ❌ "Vui lòng chọn model cần xóa!"
- **Nguyên nhân:** Chưa chọn model nào trong ComboBox
- **Giải pháp:** Chọn model từ ComboBox trước khi click "Del"

### ⚠️ Buttons "Add" và "Del" không hiển thị
- **Nguyên nhân:** Đang ở Jog Mode
- **Giải pháp:** Chuyển sang Teaching Mode

### ⚠️ Buttons "Add" và "Del" biến mất
- **Nguyên nhân:** Vừa chuyển Port hoặc chuyển sang Jog Mode
- **Giải pháp:**
  - Khi chuyển Port: Tự động về Jog Mode (thiết kế an toàn)
  - Cần teaching lại: Chuyển sang Teaching Mode

## File Lưu Trữ

### Vị trí file
```
[Thư mục ứng dụng]/TeachingModels/teaching_models.json
```

Ví dụ:
```
C:\Users\ngvan\Projects\PC\PLCKeygen\PLCKeygen\bin\Debug\TeachingModels\teaching_models.json
```

### Cấu trúc file JSON

```json
{
  "Models": [
    {
      "ModelName": "Model_ABC",
      "CreatedDate": "2025-01-08T10:30:00",
      "LastModified": "2025-01-08T10:30:00",
      "Description": "",
      "Port1": {
        "TrayInputXYStart": { "X": 12000, "Y": 15000, "Z": 0, ... },
        "TrayInputXEnd": { "X": 45000, "Y": 15000, "Z": 0, ... },
        ...
      },
      "Port2": { ... },
      "Port3": { ... },
      "Port4": { ... }
    }
  ]
}
```

### Sao lưu & Phục hồi

**Sao lưu:**
1. Copy file `teaching_models.json`
2. Lưu vào USB/Cloud

**Phục hồi:**
1. Copy file backup vào thư mục `TeachingModels`
2. Khởi động lại ứng dụng
3. Models sẽ tự động load

## Tips & Best Practices

### ✅ Nên làm

1. **Đặt tên model rõ ràng:**
   - Tốt: `Model_ABC_Rev1_2024`
   - Tránh: `Model1`, `Test`

2. **Sao lưu định kỳ:**
   - Copy file JSON ra USB/Cloud mỗi tuần

3. **Kiểm tra sau khi load:**
   - Sau khi load model, kiểm tra lại 1-2 điểm quan trọng

4. **Teaching đầy đủ:**
   - Teaching tất cả 4 ports trước khi Save model
   - Kiểm tra tất cả buttons Save đã chuyển xanh

### ❌ Tránh làm

1. **KHÔNG edit trực tiếp file JSON:**
   - Có thể làm hỏng cấu trúc
   - Dùng chức năng Add/Delete trong app

2. **KHÔNG load model khi đang chạy:**
   - Dừng máy trước
   - Chuyển sang Teaching Mode
   - Load model
   - Kiểm tra lại
   - Mới bắt đầu chạy

3. **KHÔNG dùng tên model trùng:**
   - Mỗi model cần tên riêng biệt

## Câu Hỏi Thường Gặp (FAQ)

**Q: Tôi có thể load model khi đang ở Jog Mode không?**
A: Không. Hệ thống sẽ hiển thị cảnh báo yêu cầu chuyển sang Teaching Mode.

**Q: Nếu tôi vô tình xóa model thì có phục hồi được không?**
A: Có, nếu bạn có backup file JSON. Copy file backup vào thư mục TeachingModels.

**Q: Tôi có thể export 1 model riêng lẻ không?**
A: Hiện tại chưa có tính năng này. Tất cả models được lưu chung trong 1 file JSON.

**Q: Load model có ghi đè teaching points hiện tại không?**
A: Có. Vì vậy hệ thống luôn hỏi xác nhận trước khi load.

**Q: Buttons Add/Del biến mất khi tôi chuyển Port?**
A: Đúng. Đây là thiết kế an toàn. Khi chuyển Port, hệ thống tự động về Jog Mode.

**Q: Tôi cần lưu lại model sau khi sửa teaching point không?**
A: Có. Nếu bạn sửa teaching points và muốn lưu lại, cần click "Add" lại. Hệ thống sẽ update model (nếu tên trùng) hoặc tạo model mới.

**Q: File JSON lưu ở đâu?**
A: Trong thư mục `TeachingModels` cùng cấp với file .exe của ứng dụng.

## Tóm Tắt

✅ **Teaching Mode** → Buttons hiển thị → Có thể Add/Delete/Load model
✅ **Jog Mode** → Buttons ẩn → Chỉ xem danh sách model
✅ **Chuyển Port** → Tự động về Jog Mode (an toàn)
✅ **Load model** → Luôn có xác nhận → Tránh thao tác nhầm
✅ **Delete model** → Luôn có xác nhận → Tránh xóa nhầm

---
**Lưu ý:** Tính năng này giúp quản lý teaching points cho nhiều model sản phẩm khác nhau một cách an toàn và hiệu quả. Luôn sao lưu file JSON định kỳ!
