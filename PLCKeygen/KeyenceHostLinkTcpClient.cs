using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PLCKeygen
{
    public class PLCKeyence : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private  object _lock = new object();
        private  object _lockSETBIT = new object();
        private  object _lockRESETBIT = new object();
        private  object _lockWrite = new object();
        private  object _lockRead = new object();
        private object _lockRead32 = new object();
        private object _lockWrite32 = new object();
        private bool _isConnected = false;
        private object _lockReadS16 = new object();
        private object _lockWriteS16 = new object();
        protected virtual void PropertyChangedEvent(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private TcpClient _client;
        private NetworkStream _stream;
        private  string _ip;
        private  int _port;
        private  System.Timers.Timer _timer = new System.Timers.Timer();
        private event EventHandler _reconnect;
        public bool IsSessionStarted { get; private set; } = true;

        // Timeout settings
        private const int CONNECTION_TIMEOUT_MS = 2000;  // 2 seconds for connection
        private const int READ_WRITE_TIMEOUT_MS = 1000;  // 1 second for read/write
        private const int SLOW_RESPONSE_THRESHOLD_MS = 500;  // Consider slow if > 500ms

        // Response time tracking
        private System.Diagnostics.Stopwatch _responseTimer = new System.Diagnostics.Stopwatch();
        private int _slowResponseCount = 0;
        private const int MAX_SLOW_RESPONSES = 3;  // Disconnect after 3 slow responses

        public PLCKeyence(string ipAddress, int port)
        {
            _ip = ipAddress;
            _port = port;
            _timer.Interval = 3000;
            _timer.AutoReset = false;
        }

        public void Open()
        {
            try
            {
                // Create TcpClient with connection timeout
                _client = new TcpClient();
                _client.ReceiveTimeout = READ_WRITE_TIMEOUT_MS;
                _client.SendTimeout = READ_WRITE_TIMEOUT_MS;

                // Connect with timeout using async method
                var result = _client.BeginConnect(_ip, _port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(CONNECTION_TIMEOUT_MS));

                if (!success)
                {
                    _client.Close();
                    throw new Exception($"Connection timeout after {CONNECTION_TIMEOUT_MS}ms");
                }

                _client.EndConnect(result);

                // Get stream and set timeouts
                _stream = _client.GetStream();
                _stream.ReadTimeout = READ_WRITE_TIMEOUT_MS;
                _stream.WriteTimeout = READ_WRITE_TIMEOUT_MS;

                // Reset slow response counter
                _slowResponseCount = 0;

                IsSessionStarted = true;  // Only set true if connection succeeds
            }
            catch (Exception ex)
            {
                IsSessionStarted = false;  // Ensure session is marked as not started
                PropertyChangedEvent($"{Tcpstatus.disconnected}");
                Console.WriteLine($"[HostLinkTCP] Connection failed: {ex.Message}");

                // Clean up
                _stream?.Close();
                _client?.Close();
            }
        }

        public void Close()
        {
           
            _stream?.Close();
            _client?.Close();
            PropertyChangedEvent($"{Tcpstatus.disconnected}");
            IsSessionStarted = false;
        }

        public string SendCommand(string command)
        {
            lock (_lock)
            {
                if (!IsSessionStarted || _stream == null)
                {
                    return "EX:Client is not connected";
                }

                try
                {
                    // Start measuring response time
                    _responseTimer.Restart();

                    string fullCommand = command.Trim() + "\r";
                    byte[] sendBytes = Encoding.ASCII.GetBytes(fullCommand);
                    _stream.Write(sendBytes, 0, sendBytes.Length);

                    byte[] buffer = new byte[256];
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    string raw = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                    // Stop measuring response time
                    _responseTimer.Stop();
                    long responseTimeMs = _responseTimer.ElapsedMilliseconds;

                    // Check if response is slow
                    if (responseTimeMs > SLOW_RESPONSE_THRESHOLD_MS)
                    {
                        _slowResponseCount++;
                        Console.WriteLine($"[HostLinkTCP] Slow response: {responseTimeMs}ms (count: {_slowResponseCount}/{MAX_SLOW_RESPONSES})");

                        // Disconnect if too many slow responses
                        if (_slowResponseCount >= MAX_SLOW_RESPONSES)
                        {
                            Console.WriteLine($"[HostLinkTCP] Too many slow responses, disconnecting...");
                            IsSessionStarted = false;
                            PropertyChangedEvent($"{Tcpstatus.disconnected}");
                            Close();
                            return "EX:Connection too slow";
                        }
                    }
                    else
                    {
                        // Reset counter on good response
                        if (_slowResponseCount > 0)
                        {
                            _slowResponseCount--;
                        }
                    }

                    // Tách phản hồi thành từng phần (status và data nếu có)
                    var parts = raw.Split(new[] { '\r', '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) return "EX:NO RESPONSE";

                    string status = parts[0];
                    string data = parts.Length > 1 ? parts[1] : null;

                    // Nếu không có data, nhưng status OK thì vẫn trả OK, không log lỗi
                    if (data == null) return status;

                    return data;
                }
                catch (System.IO.IOException ioEx) when (ioEx.InnerException is System.Net.Sockets.SocketException)
                {
                    // Network error or timeout
                    Console.WriteLine($"[HostLinkTCP] Network error: {ioEx.Message}");
                    IsSessionStarted = false;
                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    return $"EX:{ioEx.Message}";
                }
                catch (System.IO.IOException ioEx)
                {
                    // Read/Write timeout
                    Console.WriteLine($"[HostLinkTCP] Timeout: {ioEx.Message}");
                    _slowResponseCount++;

                    if (_slowResponseCount >= MAX_SLOW_RESPONSES)
                    {
                        IsSessionStarted = false;
                        PropertyChangedEvent($"{Tcpstatus.disconnected}");
                        Close();
                    }
                    return $"EX:Timeout";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[HostLinkTCP] Error sending command: {ex.Message}");
                    IsSessionStarted = false;
                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    return $"EX:{ex.Message}";
                }
            }
        }

        private TcpReceiveCMD ParseResponse(string response)
        {
            if (response == "OK" || response == "CC" || response == "00000") return TcpReceiveCMD.OK;
            if (response == "E0") return TcpReceiveCMD.E0;
            if (response == "E1") return TcpReceiveCMD.E1;
            if (response.StartsWith("EX:")) return TcpReceiveCMD.CheckCodeWrong;
            return TcpReceiveCMD.Unknow;
        }

        public  bool StartCommunication()
        {
            string response =  SendCommand("CR");
            var result = ParseResponse(response);
            IsSessionStarted = result == TcpReceiveCMD.OK;
            return IsSessionStarted;
        }

        public bool WriteUInt16(string address, ushort value)
        {
            lock (_lockWrite)
            {

                string response = SendCommand($"WR {address}.U {value}");
                if (response.Contains("not") || response.Contains("EX"))
                {

                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    IsSessionStarted = false;
                    if (_isConnected)
                    {

                        _timer.Elapsed -= Reconnect;
                        _timer.Elapsed += Reconnect;
                        _timer.Start();

                    }
                }
                else
                {
                    _isConnected = true;
                    _timer.Elapsed -= Reconnect;
                    _timer.Stop();
                    PropertyChangedEvent($"{Tcpstatus.connected}");
                    IsSessionStarted = true;
                }

                return ParseResponse(response) == TcpReceiveCMD.OK;
            }

        }
        public bool WriteInt32(string address, Int32 value)
        {
            lock (_lockWrite32)
            {

                string response = SendCommand($"WR {address}.L {value}");
                if (response.Contains("not") || response.Contains("EX"))
                {

                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    IsSessionStarted = false;
                    if (_isConnected)
                    {

                        _timer.Elapsed -= Reconnect;
                        _timer.Elapsed += Reconnect;
                        _timer.Start();

                    }
                }
                else
                {
                    _isConnected = true;
                    _timer.Elapsed -= Reconnect;
                    _timer.Stop();
                    PropertyChangedEvent($"{Tcpstatus.connected}");
                    IsSessionStarted = true;
                }

                return ParseResponse(response) == TcpReceiveCMD.OK;
            }

        }

        //public bool WriteInt16(string address, short value)
        //{
        //    string response = SendCommand($"WR {address}.S {value}");
        //    return ParseResponse(response) == TcpReceiveCMD.OK;
        //}

        public ushort ReadUInt16(string address)
        {
            lock (_lockRead)
            {
                try
                {

                    string response = SendCommand($"RD {address}.U");

                    if (ushort.TryParse(response, out ushort value))
                    {
                        _isConnected = true;
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        _timer.Elapsed -= Reconnect;
                        _timer.Stop();
                        return value;
                    }


                    if (response == "OK" || response == "00000")
                    {

                        MessageBox.Show($"Lỗi đọc {address}: Không có dữ liệu trả về", "PLC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }


                    if (response.Contains("not") || response.Contains("EX"))
                    {
                        Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {response}");
                        PropertyChangedEvent($"{Tcpstatus.disconnected}");
                        if (_isConnected)
                        {

                            _timer.Elapsed -= Reconnect;
                            _timer.Elapsed += Reconnect;
                            _timer.Start();

                        }



                    }
                    else
                    {
                        _isConnected = true;
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        IsSessionStarted = true;
                    }
                    return 0;


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {ex.Message}");
                    IsSessionStarted = false;
                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    return 0;
                }
                //MessageBox.Show("4");
            }

        }
        public short ReadInt16(string address)
        {
            lock (_lockReadS16)
            {
                try
                {

                    string response = SendCommand($"RD {address}.S");

                    if (short.TryParse(response, out short value))
                    {
                        _isConnected = true;
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        _timer.Elapsed -= Reconnect;
                        _timer.Stop();
                        return value;
                    }


                    if (response == "OK" || response == "00000")
                    {

                        MessageBox.Show($"Lỗi đọc {address}: Không có dữ liệu trả về", "PLC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }


                    if (response.Contains("not") || response.Contains("EX"))
                    {
                        Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {response}");
                        PropertyChangedEvent($"{Tcpstatus.disconnected}");
                        if (_isConnected)
                        {

                            _timer.Elapsed -= Reconnect;
                            _timer.Elapsed += Reconnect;
                            _timer.Start();

                        }



                    }
                    else
                    {
                        _isConnected = true;
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        IsSessionStarted = true;
                    }
                    return 0;


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {ex.Message}");
                    IsSessionStarted = false;
                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    return 0;
                }
                //MessageBox.Show("4");
            }

        }
        public bool WriteInt16(string address,short value)
        {
            lock (_lockWriteS16)
            {
                try
                {

                    string response = SendCommand($"WR {address}.S {value}");
                    if (response.Contains("not") || response.Contains("EX"))
                    {

                        PropertyChangedEvent($"{Tcpstatus.disconnected}");
                        IsSessionStarted = false;
                        if (_isConnected)
                        {

                            _timer.Elapsed -= Reconnect;
                            _timer.Elapsed += Reconnect;
                            _timer.Start();

                        }
                    }
                    else
                    {
                        _isConnected = true;
                        _timer.Elapsed -= Reconnect;
                        _timer.Stop();
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        IsSessionStarted = true;
                    }

                    return ParseResponse(response) == TcpReceiveCMD.OK;
                }
                catch
                {
                    return false;
                }
                //MessageBox.Show("4");
            }

        }
        public Int32 ReadInt32(string address)
        {
            lock (_lockRead32)
            {
                try
                {

                    string response = SendCommand($"RD {address}.L");

                    if (Int32.TryParse(response, out Int32 value))
                    {
                        _isConnected = true;
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        _timer.Elapsed -= Reconnect;
                        _timer.Stop();
                        return value;
                    }


                    if (response == "OK" || response == "00000")
                    {

                        MessageBox.Show($"Lỗi đọc {address}: Không có dữ liệu trả về", "PLC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }


                    if (response.Contains("not") || response.Contains("EX"))
                    {
                        Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {response}");
                        PropertyChangedEvent($"{Tcpstatus.disconnected}");
                        if (_isConnected)
                        {

                            _timer.Elapsed -= Reconnect;
                            _timer.Elapsed += Reconnect;
                            _timer.Start();

                        }



                    }
                    else
                    {
                        _isConnected = true;
                        PropertyChangedEvent($"{Tcpstatus.connected}");
                        IsSessionStarted = true;
                    }
                    return 0;


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi kết nối đến PLC tại địa chỉ {address}: {ex.Message}");
                    IsSessionStarted = false;
                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                    return 0;
                }
                //MessageBox.Show("4");
            }

        }




        //public short ReadInt16(string address)
        //{
        //    string response = SendCommand($"RD {address}.S");
        //    if (short.TryParse(response, out short value))
        //        return value;

        //    LogProgram.WriteLog($"[PLC] Giá trị không hợp lệ từ {address}: {response}");
        //    return 0;
        //}

        public bool ReadBit(string address)
        {
            string response = SendCommand($"RD {address}");
            if (response == "1") return true;
            if (response == "0") return false;

            //LogProgram.WriteLog($"[PLC] Bit không hợp lệ tại {address}: {response}");
            return false;
        }

        public bool SetBit(string address)
        {
            string response = SendCommand($"ST {address}");
            return ParseResponse(response) == TcpReceiveCMD.OK;
        }

        public bool ResetBit(string address)
        {
            string response = SendCommand($"RS {address}");
            return ParseResponse(response) == TcpReceiveCMD.OK;
        }

        public bool ReadBitFromWord(string wordAddress, int bitIndex)
        {
         
            if (bitIndex < 0 || bitIndex > 15)
            {
                //LogProgram.WriteLog($"[PLC] Bit index {bitIndex} không hợp lệ (0–15)");
                return false;
            }

            ushort value = ReadUInt16(wordAddress);
            bool result = (value & (1 << bitIndex)) != 0;

            // LogProgram.WriteLog($"[PLC] ReadBit: {wordAddress}.{bitIndex} = {(result ? "1" : "0")}");
            return result;
        }

        public bool SetBitInWord(string wordAddress, int bitIndex)
        {
            lock (_lockSETBIT)
            {
                ushort original = ReadUInt16(wordAddress);
                ushort updated = (ushort)(original | (1 << bitIndex));
                return WriteUInt16(wordAddress, updated);
            }
        }

        public bool ResetBitInWord(string wordAddress, int bitIndex)
        {
            lock (_lockRESETBIT)
            {
                ushort original = ReadUInt16(wordAddress);

                ushort updated = (ushort)(original & ~(1 << bitIndex));

                return WriteUInt16(wordAddress, updated);
            }
        }
        private void Reconnect(object sender, System.Timers.ElapsedEventArgs e)
        {
            
               
                try
                {
                   
                    Close();
                    Open();
                    StartCommunication();
                  //  LogProgram.WriteLog($"Ready communication to {_ip}:{_port} after Reconnect");
                 
                 

                }
                catch (Exception ex)
                {

                 Console.WriteLine($"Reconnection failed: {ex.Message}");
                _timer.Interval = 3000; // Thử lại sau 3 giây
                    _timer.AutoReset = false; // Chỉ chạy một lần
                    _timer.Elapsed -= Reconnect; // Đảm bảo không đăng ký nhiều lần
                    _timer.Elapsed += Reconnect; // Đăng ký lại sự kiện
                    _timer.Start();   
                    PropertyChangedEvent($"{Tcpstatus.disconnected}");
                   // LogProgram.WriteLog($"Reconnection failed: {ex.Message}");
                }
         
        }
    }

    public enum TcpReceiveCMD
    {
        OK,
        E0,
        E1,
        Unknow,
        CheckCodeWrong
    }
    public enum Tcpstatus
    {
        connected,
        disconnected,
    }
}
