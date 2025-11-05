# Single Instance Control - Hướng Dẫn

## Tổng Quan

PLCKeygen được thiết kế để **chỉ cho phép chạy 1 instance duy nhất** trên mỗi máy tính. Tính năng này ngăn chặn việc mở nhiều phần mềm cùng lúc, tránh xung đột khi điều khiển PLC.

## Cách Hoạt Động

### Khi Mở Phần Mềm Lần Đầu

1. Application khởi động bình thường
2. Tạo một Mutex (Mutual Exclusion) với tên unique
3. Mutex này đánh dấu rằng phần mềm đang chạy

### Khi Cố Mở Phần Mềm Lần Thứ 2

1. Application phát hiện Mutex đã tồn tại
2. Hiển thị MessageBox thông báo:
   ```
   PLCKeygen đã đang chạy!

   Chỉ được phép chạy 1 phần mềm trên 1 máy tính.
   ```
3. Tự động tìm cửa sổ đang chạy
4. Đưa cửa sổ đó lên foreground (nếu đang minimize thì restore)
5. Đóng instance mới (không mở)

## Lợi Ích

### ✅ Ngăn Chặn Xung Đột
- Tránh 2 instance cùng điều khiển PLC
- Ngăn chặn ghi đè dữ liệu teaching points
- Tránh conflict khi kết nối TCP/IP với PLC

### ✅ Tăng Độ An Toàn
- Đảm bảo chỉ 1 operator điều khiển máy tại 1 thời điểm
- Tránh nhầm lẫn giữa các cửa sổ
- Giảm thiểu rủi ro thao tác sai

### ✅ User Experience Tốt Hơn
- Tự động focus vào cửa sổ đang mở
- Thông báo rõ ràng, dễ hiểu
- Không cần phải tự tìm cửa sổ đang mở

## Technical Details

### Mutex

**Mutex Name**: `PLCKeygen_SingleInstance_Mutex_E4F8A2C1`

Mutex là một synchronization primitive trong Windows, đảm bảo chỉ có 1 process có thể "sở hữu" nó tại 1 thời điểm.

### Win32 API

Sử dụng 3 Win32 API functions:

1. **SetForegroundWindow()**
   - Đưa cửa sổ lên foreground
   - Activate window

2. **ShowWindow()**
   - Restore window từ minimized state
   - Parameter: SW_RESTORE = 9

3. **IsIconic()**
   - Kiểm tra window có đang minimize không
   - Returns true nếu minimized

### Process Detection

```csharp
// Tìm tất cả processes với tên giống nhau
Process[] processes = Process.GetProcessesByName(processName);

// Lặp qua và tìm main window handle
foreach (Process process in processes)
{
    IntPtr hWnd = process.MainWindowHandle;
    if (hWnd != IntPtr.Zero)
    {
        // Focus vào window này
    }
}
```

## Scenarios

### Scenario 1: Người Dùng Quên Đã Mở Phần Mềm

**Tình huống**:
- Operator đã mở PLCKeygen
- Minimize window
- Quên và double-click icon lại

**Kết quả**:
- MessageBox xuất hiện: "PLCKeygen đã đang chạy!"
- Cửa sổ cũ tự động restore và focus
- Operator tiếp tục làm việc với cửa sổ đó

### Scenario 2: 2 Người Cùng Muốn Dùng 1 Máy

**Tình huống**:
- Operator A đang dùng PLCKeygen
- Operator B muốn mở phần mềm

**Kết quả**:
- MessageBox thông báo đã có người đang dùng
- Cửa sổ của Operator A được focus
- Operator B biết cần đợi hoặc yêu cầu A đóng phần mềm

### Scenario 3: Phần Mềm Đóng Bất Thường

**Tình huống**:
- PLCKeygen crash hoặc bị kill
- Mutex tự động được release
- Người dùng mở lại phần mềm

**Kết quả**:
- Phần mềm mở bình thường
- Không bị lock

## Limitations

### ⚠️ Lưu Ý

1. **Chỉ áp dụng trên cùng 1 máy tính**
   - Không ngăn chặn 2 máy khác nhau cùng kết nối PLC
   - Cần có cơ chế khác để handle multi-machine scenario

2. **Mutex tự động release khi crash**
   - Nếu application crash, Mutex sẽ bị release
   - Process tiếp theo có thể chạy bình thường
   - Điều này là tốt (không bị deadlock)

3. **Requires administrator rights (không)**
   - Mutex user-level, không cần admin
   - Mỗi user account có Mutex riêng
   - User A và User B trên cùng máy có thể chạy riêng instance

## Troubleshooting

### Phần mềm không mở được dù chưa có instance nào chạy?

**Nguyên nhân**: Mutex orphaned (rất hiếm)

**Giải pháp**:
1. Mở Task Manager
2. Tìm process `PLCKeygen.exe`
3. Kill tất cả các process
4. Mở lại phần mềm

### Muốn chạy nhiều instance cho testing?

**Không khuyến khích**, nhưng có thể:

1. **Cách 1**: Đổi tên file .exe
   - Copy `PLCKeygen.exe` thành `PLCKeygen_Test.exe`
   - Chạy file mới (process name khác)

2. **Cách 2**: Comment code trong Program.cs
   - Comment toàn bộ single instance check
   - Rebuild

3. **Cách 3**: Run as different user
   - Mỗi user account có Mutex riêng

### Cửa sổ không tự động focus?

**Nguyên nhân**: Windows security policy

**Giải pháp**:
- Click vào taskbar icon
- Hoặc Alt+Tab để switch

## Code Structure

### Program.cs

```csharp
[STAThread]
static void Main()
{
    // 1. Tạo mutex
    bool createdNew;
    mutex = new Mutex(true, MUTEX_NAME, out createdNew);

    // 2. Kiểm tra đã có instance chưa
    if (!createdNew)
    {
        // Thông báo và focus cửa sổ cũ
        MessageBox.Show(...);
        BringExistingInstanceToFront();
        return;
    }

    // 3. Chạy application bình thường
    try
    {
        Application.Run(new Form1());
    }
    finally
    {
        // 4. Release mutex khi đóng
        mutex.ReleaseMutex();
    }
}
```

### Mutex Lifecycle

```
┌─────────────────────────────────────────┐
│  PLCKeygen.exe started                  │
└─────────────────┬───────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────┐
│  Create Mutex (createdNew = ?)          │
└─────────┬───────────────────┬───────────┘
          │                   │
    createdNew=true     createdNew=false
          │                   │
          ▼                   ▼
┌─────────────────┐   ┌──────────────────┐
│ Run Form1()     │   │ Show MessageBox  │
│                 │   │ Focus old window │
│ (Normal run)    │   │ Exit             │
└────────┬────────┘   └──────────────────┘
         │
         ▼
┌─────────────────┐
│ User closes app │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Release Mutex   │
└─────────────────┘
```

## Best Practices

### ✅ Khuyến Nghị

1. **Đóng phần mềm đúng cách**
   - Dùng nút X hoặc File → Exit
   - Tránh Force Close từ Task Manager

2. **Kiểm tra trước khi mở**
   - Nhìn vào taskbar xem có icon PLCKeygen không
   - Tránh double-click nhiều lần

3. **Communication giữa operators**
   - Thông báo khi đang dùng phần mềm
   - Đóng phần mềm khi không dùng nữa

---

## Summary

Tính năng Single Instance đảm bảo:
- ✅ Chỉ 1 instance chạy trên 1 máy
- ✅ Tự động focus vào instance đang chạy
- ✅ Thông báo rõ ràng cho user
- ✅ Tránh conflict khi điều khiển PLC
- ✅ Tăng độ an toàn và ổn định

Version: PLCKeygen 2025.11.2+
