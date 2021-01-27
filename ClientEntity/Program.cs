using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientEntity
{
    internal static class Program
    {
        private const string Host = "127.0.0.1";
        private const int Port = 8888;
        private static string _userName;
        private static TcpClient _client;
        private static NetworkStream _stream;

        private static void Main()
        {
            Console.Write("Enter your name: ");
            _userName = Console.ReadLine();
            _client = new TcpClient();
            try
            {
                _client.Connect(Host, Port);
                _stream = _client.GetStream();

                var message = _userName;
                var data = Encoding.Unicode.GetBytes(message ?? throw new InvalidOperationException());
                _stream.Write(data, 0, data.Length);


                var receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start();
                Console.WriteLine($"Hi, {_userName}, what's up");
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        private static void SendMessage()
        {
            Console.WriteLine("Enter message: ");

            while (true)
            {
                var message = Console.ReadLine();
                var data = Encoding.Unicode.GetBytes(message ?? throw new InvalidOperationException());
                _stream.Write(data, 0, data.Length);
            }
        }

        private static void ReceiveMessage()
        {
            while (true)
                try
                {
                    var data = new byte[64];
                    var builder = new StringBuilder();
                    do
                    {
                        var bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (_stream.DataAvailable);

                    var message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Connection was suspected!");
                    Console.ReadLine();
                    Disconnect();
                }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void Disconnect()
        {
            _stream?.Close();
            _client?.Close();
            Environment.Exit(0);
        }
    }
}