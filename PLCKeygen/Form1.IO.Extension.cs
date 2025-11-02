using System;
using System.Drawing;
using System.Windows.Forms;

namespace PLCKeygen
{
    /// <summary>
    /// Form1 extension methods for I/O monitoring and control
    /// Add these methods to your Form1.cs class or use as a partial class
    /// </summary>
    public partial class Form1
    {
        // Add these fields to your Form1 class
        private bool previousEMS1 = false;
        private bool previousEMS2 = false;
        private bool previousEMS3 = false;
        private bool previousEMS4 = false;

        /// <summary>
        /// Call this method in your timer1_Tick to monitor I/O status
        /// Add this line to timer1_Tick: UpdateIOStatus();
        /// </summary>
        private void UpdateIOStatus()
        {
            try
            {
                // Monitor safety status
                MonitorEmergencyStops();

                // Monitor quality status
                MonitorQualityStatus();

                // Monitor part presence
                MonitorPartPresence();

                // Monitor fixture status
                MonitorFixtureStatus();
            }
            catch (Exception ex)
            {
                // Handle errors silently or log
                Console.WriteLine($"I/O Update Error: {ex.Message}");
            }
        }

        #region Emergency Stop Monitoring

        /// <summary>
        /// Monitor emergency stops and alert when pressed
        /// </summary>
        private void MonitorEmergencyStops()
        {
            bool ems1 = PLCKey.ReadBit(PLCAddresses.Input.P1_SW_EMS1);
            bool ems2 = PLCKey.ReadBit(PLCAddresses.Input.P2_SW_EMS2);
            bool ems3 = PLCKey.ReadBit(PLCAddresses.Input.P3_SW_EMS3);
            bool ems4 = PLCKey.ReadBit(PLCAddresses.Input.P4_SW_EMS4);

            // Check if any EMS was just pressed
            if ((ems1 && !previousEMS1) || (ems2 && !previousEMS2) || (ems3 && !previousEMS3) || (ems4 && !previousEMS4))
            {
                // EMS was just activated - alert user
                OnEmergencyStopActivated(ems1, ems2, ems3, ems4);
            }

            // Update previous states
            previousEMS1 = ems1;
            previousEMS2 = ems2;
            previousEMS3 = ems3;
            previousEMS4 = ems4;

            // Update status display (if you have a status label)
            UpdateEMSStatusDisplay(ems1, ems2, ems3, ems4);
        }

        private void OnEmergencyStopActivated(bool port1, bool port2, bool port3, bool port4)
        {
            string message = "EMERGENCY STOP ACTIVATED!\n\n";
            if (port1) message += "• Port 1\n";
            if (port2) message += "• Port 2\n";
            if (port3) message += "• Port 3\n";
            if (port4) message += "• Port 4\n";

            MessageBox.Show(message, "EMERGENCY STOP", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Set all tower lights to RED using Output addresses
            if (port1) PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Red);
            if (port2) PLCKey.SetBit(PLCAddresses.Output.P2_Tower_Red);
            if (port3) PLCKey.SetBit(PLCAddresses.Output.P3_Tower_Red);
            if (port4) PLCKey.SetBit(PLCAddresses.Output.P4_Tower_Red);
        }

        private void UpdateEMSStatusDisplay(bool ems1, bool ems2, bool ems3, bool ems4 = false)
        {
            // If you have status labels/indicators on your form, update them here
            // Example:
            // lblEMSPort1.BackColor = ems1 ? Color.Red : Color.Green;
            // lblEMSPort2.BackColor = ems2 ? Color.Red : Color.Green;
            // lblEMSPort3.BackColor = ems3 ? Color.Red : Color.Green;
        }

        #endregion

        #region Quality Monitoring

        /// <summary>
        /// Monitor quality sensors and update tower lights
        /// </summary>
        private void MonitorQualityStatus()
        {
            // Port 1 Quality
            bool p1_jigOK = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_OK);
            bool p1_jigNG = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_NG);

            // Port 2 Quality
            bool p2_jigOK = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Jig_OK);
            bool p2_jigNG = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Jig_NG);

            // Port 3 Quality
            bool p3_jigOK = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Jig_OK);
            bool p3_jigNG = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Jig_NG);

            // Update quality indicators (if you have labels/controls)
            UpdateQualityDisplay(1, p1_jigOK, p1_jigNG);
            UpdateQualityDisplay(2, p2_jigOK, p2_jigNG);
            UpdateQualityDisplay(3, p3_jigOK, p3_jigNG);
        }

        private void UpdateQualityDisplay(int port, bool jigOK, bool jigNG)
        {
            // Example: Update labels or controls based on quality status
            // You can add labels to your form: lblQualityPort1, lblQualityPort2, lblQualityPort3
            /*
            Label qualityLabel = null;
            switch (port)
            {
                case 1: qualityLabel = lblQualityPort1; break;
                case 2: qualityLabel = lblQualityPort2; break;
                case 3: qualityLabel = lblQualityPort3; break;
            }

            if (qualityLabel != null)
            {
                if (jigOK)
                {
                    qualityLabel.Text = "OK";
                    qualityLabel.BackColor = Color.LightGreen;
                }
                else if (jigNG)
                {
                    qualityLabel.Text = "NG";
                    qualityLabel.BackColor = Color.LightCoral;
                }
                else
                {
                    qualityLabel.Text = "--";
                    qualityLabel.BackColor = Color.LightGray;
                }
            }
            */
        }

        #endregion

        #region Part Presence Monitoring

        /// <summary>
        /// Monitor part presence sensors
        /// </summary>
        private void MonitorPartPresence()
        {
            bool p1_in = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_VIn1);
            bool p1_out = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_VOt1);

            bool p2_in = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_VIn2);
            bool p2_out = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_VOt2);

            bool p3_in = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_VIn3);
            bool p3_out = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_VOt3);

            // Update part status display
            UpdatePartPresenceDisplay(1, p1_in, p1_out);
            UpdatePartPresenceDisplay(2, p2_in, p2_out);
            UpdatePartPresenceDisplay(3, p3_in, p3_out);
        }

        private void UpdatePartPresenceDisplay(int port, bool partIn, bool partOut)
        {
            // Example: Update status indicators
            // Add controls to your form if needed
        }

        #endregion

        #region Fixture Status Monitoring

        /// <summary>
        /// Monitor fixture open/close status
        /// </summary>
        private void MonitorFixtureStatus()
        {
            // Port 1
            bool p1_open = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Fix1_Open);
            bool p1_close = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Fix1_Close);

            // Port 2
            bool p2_open = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Fix2_Open);
            bool p2_close = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Fix2_Close);

            // Port 3
            bool p3_open = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Fix3_Open);
            bool p3_close = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Fix3_Close);

            UpdateFixtureDisplay(1, p1_open, p1_close);
            UpdateFixtureDisplay(2, p2_open, p2_close);
            UpdateFixtureDisplay(3, p3_open, p3_close);
        }

        private void UpdateFixtureDisplay(int port, bool open, bool close)
        {
            // Example: Update fixture status indicators
        }

        #endregion

        #region Tower Light Control Methods

        /// <summary>
        /// Set tower light for a port
        /// </summary>
        /// <param name="port">Port number (1-3)</param>
        /// <param name="color">Color: GREEN, YELLOW, RED, or OFF</param>
        public void SetTowerLight(int port, string color)
        {
            // Turn off all lights first
            TurnOffTowerLights(port);

            // Turn on requested light using Output addresses
            if (color.ToUpper() == "GREEN")
            {
                switch (port)
                {
                    case 1: PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Green); break;
                    case 2: PLCKey.SetBit(PLCAddresses.Output.P2_Tower_Green); break;
                    case 3: PLCKey.SetBit(PLCAddresses.Output.P3_Tower_Green); break;
                }
            }
            else if (color.ToUpper() == "YELLOW")
            {
                switch (port)
                {
                    case 1: PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Yellow); break;
                    case 2: PLCKey.SetBit(PLCAddresses.Output.P2_Tower_Yellow); break;
                    case 3: PLCKey.SetBit(PLCAddresses.Output.P3_Tower_Yellow); break;
                }
            }
            else if (color.ToUpper() == "RED")
            {
                switch (port)
                {
                    case 1: PLCKey.SetBit(PLCAddresses.Output.P1_Tower_Red); break;
                    case 2: PLCKey.SetBit(PLCAddresses.Output.P2_Tower_Red); break;
                    case 3: PLCKey.SetBit(PLCAddresses.Output.P3_Tower_Red); break;
                }
            }
        }

        /// <summary>
        /// Set tower light using helper method (alternative approach)
        /// </summary>
        public void SetTowerLightV2(int port, string color)
        {
            TurnOffTowerLights(port);

            if (color.ToUpper() != "OFF")
            {
                string lightAddr = PLCAddresses.Output.GetTowerLight(port, color);
                PLCKey.SetBit(lightAddr);
            }
        }

        /// <summary>
        /// Turn off all tower lights for a port
        /// </summary>
        public void TurnOffTowerLights(int port)
        {
            switch (port)
            {
                case 1:
                    PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Green);
                    PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Yellow);
                    PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Red);
                    PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Start);
                    PLCKey.ResetBit(PLCAddresses.Output.P1_Tower_Stop);
                    break;
                case 2:
                    PLCKey.ResetBit(PLCAddresses.Output.P2_Tower_Green);
                    PLCKey.ResetBit(PLCAddresses.Output.P2_Tower_Yellow);
                    PLCKey.ResetBit(PLCAddresses.Output.P2_Tower_Red);
                    PLCKey.ResetBit(PLCAddresses.Output.P2_Tower_Start);
                    PLCKey.ResetBit(PLCAddresses.Output.P2_Tower_Stop);
                    break;
                case 3:
                    PLCKey.ResetBit(PLCAddresses.Output.P3_Tower_Green);
                    PLCKey.ResetBit(PLCAddresses.Output.P3_Tower_Yellow);
                    PLCKey.ResetBit(PLCAddresses.Output.P3_Tower_Red);
                    PLCKey.ResetBit(PLCAddresses.Output.P3_Tower_Start);
                    PLCKey.ResetBit(PLCAddresses.Output.P3_Tower_Stop);
                    break;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if a port is safe to operate
        /// </summary>
        public bool IsPortSafe(int port)
        {
            bool emsOK = false;
            bool doorClosed = false;

            switch (port)
            {
                case 1:
                    emsOK = !PLCKey.ReadBit(PLCAddresses.Input.P1_SW_EMS1);
                    doorClosed = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Door1);
                    break;
                case 2:
                    emsOK = !PLCKey.ReadBit(PLCAddresses.Input.P2_SW_EMS2);
                    doorClosed = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Door2);
                    break;
                case 3:
                    emsOK = !PLCKey.ReadBit(PLCAddresses.Input.P3_SW_EMS3);
                    doorClosed = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Door3);
                    break;
            }

            return emsOK && doorClosed;
        }

        /// <summary>
        /// Get quality status for a port
        /// </summary>
        public string GetPortQualityStatus(int port)
        {
            bool jigOK = false;
            bool jigNG = false;

            switch (port)
            {
                case 1:
                    jigOK = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_OK);
                    jigNG = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Jig_NG);
                    break;
                case 2:
                    jigOK = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Jig_OK);
                    jigNG = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Jig_NG);
                    break;
                case 3:
                    jigOK = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Jig_OK);
                    jigNG = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Jig_NG);
                    break;
            }

            return jigOK ? "OK" : (jigNG ? "NG" : "UNKNOWN");
        }

        /// <summary>
        /// Get fixture status for a port
        /// </summary>
        public string GetFixtureStatus(int port)
        {
            bool open = false;
            bool close = false;

            switch (port)
            {
                case 1:
                    open = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Fix1_Open);
                    close = PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_Fix1_Close);
                    break;
                case 2:
                    open = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Fix2_Open);
                    close = PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_Fix2_Close);
                    break;
                case 3:
                    open = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Fix3_Open);
                    close = PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_Fix3_Close);
                    break;
            }

            return open ? "OPEN" : (close ? "CLOSED" : "MOVING");
        }

        /// <summary>
        /// Check if a part is present at a port
        /// </summary>
        public bool IsPartPresent(int port)
        {
            switch (port)
            {
                case 1: return PLCKey.ReadBit(PLCAddresses.Input.P1_Ss_VIn1);
                case 2: return PLCKey.ReadBit(PLCAddresses.Input.P2_Ss_VIn2);
                case 3: return PLCKey.ReadBit(PLCAddresses.Input.P3_Ss_VIn3);
                default: return false;
            }
        }

        #endregion
    }
}
