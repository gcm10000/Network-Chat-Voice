using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkChatUDP
{
    class ConnectionUDP
    {
        private const int port = 8000;
        private static IPEndPoint ip;
        public bool FormatInvalid = false;
        UdpClient udpclient;

        public ConnectionUDP(string IP)
        {
            udpclient = new UdpClient(8000, AddressFamily.InterNetwork);
            try
            {
                ip = new IPEndPoint(IPAddress.Parse(IP), port);
                FormatInvalid = false;
            }
            catch (FormatException)
            {
                FormatInvalid = true;
            }
        }
        public void Send(byte[] data, int Length)
        {
            udpclient.Send(data, Length, ip);
        }
        public void Receive(out byte[] data)
        {
            data = udpclient.Receive(ref ip);
        }
    }
}
