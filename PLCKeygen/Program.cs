using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace PLCKeygen
{
    internal static class Program
    {
        // Win32 API imports for focusing existing window
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_RESTORE = 9;

        // Mutex name - unique identifier for this application
        private static Mutex mutex = null;
        private const string MUTEX_NAME = "PLCKeygen_SingleInstance_Mutex_E4F8A2C1";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create mutex to ensure single instance
            bool createdNew;
            mutex = new Mutex(true, MUTEX_NAME, out createdNew);

            if (!createdNew)
            {
                // Application is already running, try to focus it
                MessageBox.Show(
                    "PLCKeygen đã đang chạy!\n\nChỉ được phép chạy 1 phần mềm trên 1 máy tính.",
                    "Ứng dụng đang chạy",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                // Try to find and focus the existing instance
                BringExistingInstanceToFront();

                return; // Exit this instance
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            finally
            {
                // Release mutex when application closes
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }

        /// <summary>
        /// Find and bring the existing instance to front
        /// </summary>
        private static void BringExistingInstanceToFront()
        {
            try
            {
                // Get current process name
                string processName = Process.GetCurrentProcess().ProcessName;

                // Find all processes with the same name
                Process[] processes = Process.GetProcessesByName(processName);

                foreach (Process process in processes)
                {
                    // Skip current process
                    if (process.Id == Process.GetCurrentProcess().Id)
                        continue;

                    // Get main window handle
                    IntPtr hWnd = process.MainWindowHandle;

                    if (hWnd != IntPtr.Zero)
                    {
                        // If window is minimized, restore it
                        if (IsIconic(hWnd))
                        {
                            ShowWindow(hWnd, SW_RESTORE);
                        }

                        // Bring window to front
                        SetForegroundWindow(hWnd);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail - user already got the message box
                System.Diagnostics.Debug.WriteLine($"Error bringing window to front: {ex.Message}");
            }
        }
    }
}
