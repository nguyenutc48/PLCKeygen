using System;
using System.Net.Sockets;
using System.Text;

namespace PLCKeygen
{
    public class CameraTcpClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private string _ip;
        private int _port;
        private bool _isConnected = false;

        public bool IsConnected => _isConnected;

        public CameraTcpClient(string ipAddress, int port)
        {
            _ip = ipAddress;
            _port = port;
        }

        public bool Connect()
        {
            try
            {
                _client = new TcpClient(_ip, _port);
                _stream = _client.GetStream();
                _isConnected = true;
                Console.WriteLine($"[Camera] Connected to {_ip}:{_port}");
                return true;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                Console.WriteLine($"[Camera] Connection failed: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
                _isConnected = false;
                Console.WriteLine("[Camera] Disconnected");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Camera] Error disconnecting: {ex.Message}");
            }
        }

        public string SendCommand(string command)
        {
            if (!_isConnected || _stream == null)
            {
                return "Error: Not connected";
            }

            try
            {
                // Thêm <CR><LF> vào cuối lệnh
                string fullCommand = command + "\r\n";
                byte[] sendBytes = Encoding.ASCII.GetBytes(fullCommand);
                _stream.Write(sendBytes, 0, sendBytes.Length);

                // Đọc phản hồi
                byte[] buffer = new byte[1024];
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                Console.WriteLine($"[Camera] Sent: {command}, Response: {response}");
                return response;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                Console.WriteLine($"[Camera] Error sending command: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public string SendHomeCommand()
        {
            return SendCommand("GCP,2,HOME2D,0,0,0,0,0,0");
        }
    }
}
