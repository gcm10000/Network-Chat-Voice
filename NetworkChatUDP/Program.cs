using System;
using NAudio.Wave;
using g711audio;

namespace NetworkChatUDP
{
    class Program
    {
        static BufferedWaveProvider waveProvider;
        static ConnectionUDP c;
        static void Main()
        {
            do
            {
                Console.Write("Digite o IP para conexão UDP: ");
                c = new ConnectionUDP(Console.ReadLine());
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

            //Initialize Recording
            WaveInEvent WaveIn = new WaveInEvent()
            {
                //Default buffer milliseconds for CODEC G.711 is 20ms. Can be 20ms (DEFAULT) or 30ms.
                BufferMilliseconds = 20,
                //Input Realtek High Definition
                DeviceNumber = 0,
                //Default format CD, 8 bits, mono channel
                WaveFormat = new WaveFormat(8000, 8, 1)
            };

            Console.WriteLine("Entrada de áudio selecionada: {0}", NAudio.Wave.WaveIn.GetCapabilities(0).ProductName);
            WaveIn.StartRecording();
            //WaveIn.StopRecording();
            WaveIn.DataAvailable += WaveIn_DataAvailable;

            //Initalize Playing
            waveProvider = new BufferedWaveProvider(new WaveFormat(8000, 8, 1));
            WaveOut waveOut = new WaveOut();
            waveOut.Init(waveProvider);
            waveOut.Play();

            while (true)
            {
                c.Receive(out byte[] data);
                ALawDecoder.ALawDecode(data, out byte[] decoded);
                waveProvider.AddSamples(decoded, 0, decoded.Length);
            }
        }
        private static void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] wave = ALawEncoder.ALawEncode(e.Buffer);
            c.Send(wave, wave.Length);
        }
        private static void Continue()
        {
            Console.WriteLine("Pressione alguma tecla para continuar...");
            Console.ReadKey();
        }
    }
}
