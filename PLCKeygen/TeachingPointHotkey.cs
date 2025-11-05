using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Class quản lý phím tắt cho Teaching Points
    /// </summary>
    public class TeachingPointHotkey
    {
        public Keys Key { get; set; }
        public bool Ctrl { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public string PointName { get; set; }
        public string ButtonName { get; set; }
        public string Description { get; set; }

        public TeachingPointHotkey(Keys key, string pointName, string buttonName, string description,
                                   bool ctrl = false, bool alt = false, bool shift = false)
        {
            Key = key;
            Ctrl = ctrl;
            Alt = alt;
            Shift = shift;
            PointName = pointName;
            ButtonName = buttonName;
            Description = description;
        }

        /// <summary>
        /// Kiểm tra xem key event có khớp với hotkey này không
        /// </summary>
        public bool Matches(KeyEventArgs e)
        {
            return e.KeyCode == Key &&
                   e.Control == Ctrl &&
                   e.Alt == Alt &&
                   e.Shift == Shift;
        }

        /// <summary>
        /// Lấy chuỗi mô tả hotkey (VD: "Ctrl+F1")
        /// </summary>
        public string GetHotkeyString()
        {
            var parts = new List<string>();
            if (Ctrl) parts.Add("Ctrl");
            if (Alt) parts.Add("Alt");
            if (Shift) parts.Add("Shift");
            parts.Add(Key.ToString());
            return string.Join("+", parts);
        }
    }

    /// <summary>
    /// Manager class cho tất cả teaching point hotkeys
    /// </summary>
    public class TeachingHotkeyManager
    {
        private List<TeachingPointHotkey> _hotkeys;

        public TeachingHotkeyManager()
        {
            _hotkeys = new List<TeachingPointHotkey>();
            InitializeHotkeys();
        }

        /// <summary>
        /// Khởi tạo tất cả các hotkey mapping
        /// Sử dụng Ctrl+số và Alt+số để tránh xung đột với F1-F4 (dùng để chọn Port)
        /// </summary>
        private void InitializeHotkeys()
        {
            // ===== SAVE TEACHING POINTS =====

            // Tray Input Points (4 points) - Ctrl+1 to Ctrl+4
            _hotkeys.Add(new TeachingPointHotkey(Keys.D1, "TrayInputXYStart", "btnSavePointTrayInputXYStart",
                "Tray Input - XY Start", ctrl: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D2, "TrayInputXEnd", "btnSavePointTrayInputXEnd",
                "Tray Input - X End", ctrl: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D3, "TrayInputYEnd", "btnSavePointTrayInputYEnd",
                "Tray Input - Y End", ctrl: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D4, "TrayInputZ", "btnSavePointTrayInputZ",
                "Tray Input - Z Position", ctrl: true));

            // Tray NG1 Points (4 points) - Ctrl+Alt+1 to Ctrl+Alt+4
            _hotkeys.Add(new TeachingPointHotkey(Keys.D1, "TrayNG1XYStart", "btnSavePointTrayNG1XYStart",
                "Tray NG1 - XY Start", ctrl: true, alt: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D2, "TrayNG1XEnd", "btnSavePointTrayNG1XEnd",
                "Tray NG1 - X End", ctrl: true, alt: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D3, "TrayNG1YEnd", "btnSavePointTrayNG1YEnd",
                "Tray NG1 - Y End", ctrl: true, alt: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D4, "TrayNG1Z", "btnSavePointTrayNG1Z",
                "Tray NG1 - Z Position", ctrl: true, alt: true));

            // Tray NG2 Points (4 points) - Alt+1 to Alt+4
            _hotkeys.Add(new TeachingPointHotkey(Keys.D1, "TrayNG2XYStart", "btnSavePointTrayNG2XYStart",
                "Tray NG2 - XY Start", alt: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D2, "TrayNG2XEnd", "btnSavePointTrayNG2XEnd",
                "Tray NG2 - X End", alt: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D3, "TrayNG2YEnd", "btnSavePointTrayNG2YEnd",
                "Tray NG2 - Y End", alt: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D4, "TrayNG2Z", "btnSavePointTrayNG2Z",
                "Tray NG2 - Z Position", alt: true));

            // Socket Points (6 points) - F5 to F10
            _hotkeys.Add(new TeachingPointHotkey(Keys.F5, "Socket", "btnSavePointSocket",
                "Socket - XY Position"));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F6, "SocketZLoad", "btnSavePointSocketZLoad",
                "Socket - Z Load"));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F7, "SocketZUnload", "btnSavePointSocketZUnload",
                "Socket - Z Unload"));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F8, "SocketZReady", "btnSavePointSocketZReady",
                "Socket - Z Ready"));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F9, "SocketFOpened", "btnSavePointSocketFOpened",
                "Socket - F Opened"));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F10, "SocketFClosed", "btnSavePointSocketFClosed",
                "Socket - F Closed"));

            // Camera Points (2 points) - F11, F12
            _hotkeys.Add(new TeachingPointHotkey(Keys.F11, "Camera", "btnSavePointCamera",
                "Camera - XY Position"));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F12, "SocketCameraZ", "btnSavePointSocketCameraZ",
                "Camera - Z Position"));

            // ===== GO TO TEACHING POINTS =====

            // Tray Input - Ctrl+Shift+1 to Ctrl+Shift+4
            _hotkeys.Add(new TeachingPointHotkey(Keys.D1, "TrayInputXYStart", "btnGoPointTrayInputXYStart",
                "Go to Tray Input - XY Start", ctrl: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D2, "TrayInputXEnd", "btnGoPointTrayInputXEnd",
                "Go to Tray Input - X End", ctrl: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D3, "TrayInputYEnd", "btnGoPointTrayInputYEnd",
                "Go to Tray Input - Y End", ctrl: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D4, "TrayInputZ", "btnGoPointTrayInputZ",
                "Go to Tray Input - Z", ctrl: true, shift: true));

            // Tray NG1 - Ctrl+Alt+Shift+1 to Ctrl+Alt+Shift+4
            _hotkeys.Add(new TeachingPointHotkey(Keys.D1, "TrayNG1XYStart", "btnGoPointTrayNG1XYStart",
                "Go to Tray NG1 - XY Start", ctrl: true, alt: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D2, "TrayNG1XEnd", "btnGoPointTrayNG1XEnd",
                "Go to Tray NG1 - X End", ctrl: true, alt: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D3, "TrayNG1YEnd", "btnGoPointTrayNG1YEnd",
                "Go to Tray NG1 - Y End", ctrl: true, alt: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D4, "TrayNG1Z", "btnGoPointTrayNG1Z",
                "Go to Tray NG1 - Z", ctrl: true, alt: true, shift: true));

            // Tray NG2 - Alt+Shift+1 to Alt+Shift+4
            _hotkeys.Add(new TeachingPointHotkey(Keys.D1, "TrayNG2XYStart", "btnGoPointTrayNG2XYStart",
                "Go to Tray NG2 - XY Start", alt: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D2, "TrayNG2XEnd", "btnGoPointTrayNG2XEnd",
                "Go to Tray NG2 - X End", alt: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D3, "TrayNG2YEnd", "btnGoPointTrayNG2YEnd",
                "Go to Tray NG2 - Y End", alt: true, shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.D4, "TrayNG2Z", "btnGoPointTrayNG2Z",
                "Go to Tray NG2 - Z", alt: true, shift: true));

            // Socket - Shift+F5 to Shift+F10
            _hotkeys.Add(new TeachingPointHotkey(Keys.F5, "Socket", "btnGoPointSocket",
                "Go to Socket - XY", shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F6, "SocketZLoad", "btnGoPointSocketZLoad",
                "Go to Socket - Z Load", shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F7, "SocketZUnload", "btnGoPointSocketZUnload",
                "Go to Socket - Z Unload", shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F8, "SocketZReady", "btnGoPointSocketZReady",
                "Go to Socket - Z Ready", shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F9, "SocketFOpened", "btnGoPointSocketFOpened",
                "Go to Socket - F Opened", shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F10, "SocketFClosed", "btnGoPointSocketFClosed",
                "Go to Socket - F Closed", shift: true));

            // Camera - Shift+F11, Shift+F12
            _hotkeys.Add(new TeachingPointHotkey(Keys.F11, "Camera", "btnGoPointCamera",
                "Go to Camera - XY", shift: true));
            _hotkeys.Add(new TeachingPointHotkey(Keys.F12, "SocketCameraZ", "btnGoPointSocketCameraZ",
                "Go to Camera - Z", shift: true));
        }

        /// <summary>
        /// Tìm hotkey phù hợp với key event
        /// </summary>
        public TeachingPointHotkey FindHotkey(KeyEventArgs e)
        {
            return _hotkeys.Find(h => h.Matches(e));
        }

        /// <summary>
        /// Lấy danh sách tất cả hotkeys
        /// </summary>
        public List<TeachingPointHotkey> GetAllHotkeys()
        {
            return new List<TeachingPointHotkey>(_hotkeys);
        }

        /// <summary>
        /// Lấy chuỗi help text cho tất cả hotkeys
        /// </summary>
        public string GetHelpText()
        {
            var lines = new List<string>();
            lines.Add("=== PHÍM TẮT TEACHING MODE ===\n");
            lines.Add("SAVE (Lưu điểm Teaching):");
            lines.Add("  Ctrl+1-4: Tray Input (XYStart, XEnd, YEnd, Z)");
            lines.Add("  Ctrl+Alt+1-4: Tray NG1 (XYStart, XEnd, YEnd, Z)");
            lines.Add("  Alt+1-4: Tray NG2 (XYStart, XEnd, YEnd, Z)");
            lines.Add("  F5-F10: Socket (XY, ZLoad, ZUnload, ZReady, FOpened, FClosed)");
            lines.Add("  F11-F12: Camera (XY, Z)\n");
            lines.Add("GO (Di chuyển đến điểm Teaching):");
            lines.Add("  Ctrl+Shift+1-4: Go to Tray Input");
            lines.Add("  Ctrl+Alt+Shift+1-4: Go to Tray NG1");
            lines.Add("  Alt+Shift+1-4: Go to Tray NG2");
            lines.Add("  Shift+F5-F10: Go to Socket");
            lines.Add("  Shift+F11-F12: Go to Camera\n");
            lines.Add("Lưu ý: Phím tắt chỉ hoạt động khi ở chế độ Teaching Mode");
            lines.Add("       F1-F4 được giữ để chọn Port 1-4");

            return string.Join("\n", lines);
        }
    }
}
