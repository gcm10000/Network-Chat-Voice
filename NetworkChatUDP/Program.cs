using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using NAudio.Wave;
using g711audio;

namespace NetworkChatUDP
{
    class Program
    {
        static Connection c;
        static void Main(string[] args)
        {
            do
            {
                Console.Write("Digite o IP para conexão UDP: ");
                c = new Connection(Console.ReadLine());
            }
            while (c.FormatInvalid);
            Console.WriteLine("Conectado.");
            InitializeAudio();

            Continue();

        }

        static void InitializeAudio()
        {
            //for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            //{
            //    Console.WriteLine(NAudio.Wave.WaveIn.GetCapabilities(i).ProductName);
            //}
            //deviceNumber = sourceList.SelectedItems[0].Index;

            WaveInEvent WaveIn = new WaveInEvent()
            {
                //Default buffer milliseconds for CODEC G.711 is 20ms. Can be 20ms (DEFAULT) or 30ms.
                BufferMilliseconds = 20,
                //Input Realtek High Definition
                DeviceNumber = 0,
                //Default format CD, 8 bits, mono channel
                WaveFormat = new WaveFormat(44100, 16, 1)
            };
            WaveIn.StartRecording();
            //WaveIn.StopRecording();
            WaveIn.DataAvailable += WaveIn_DataAvailable;
            Console.WriteLine("Entrada de áudio selecionada: {0}", NAudio.Wave.WaveIn.GetCapabilities(0).ProductName);
        }

        private static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] wave = ALawEncoder.ALawEncode(e.Buffer);
            c.Send(wave, wave.Length);
            //Console.WriteLine(Encoding.UTF8.GetString(e.Buffer));
            Console.WriteLine("NORMAL: " + e.BytesRecorded);
            Console.WriteLine("ENCODING: " + wave.Length);
        }

        static void Continue()
        {
            Console.WriteLine("Pressione alguma tecla para continuar...");
            Console.ReadKey();
        }
    }

    class Connection
    {
        private const int port = 8000;
        private static IPEndPoint ip;
        public bool FormatInvalid = false;
        UdpClient udpclient;

        public Connection(string IP)
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
    }
}
