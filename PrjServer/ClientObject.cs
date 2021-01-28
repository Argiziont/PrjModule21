using System;
using System.Net.Sockets;
using System.Text;

namespace TCPIPLib
{
    public class ClientObject
    {
        private readonly TcpClient _client;
        private readonly ServerObject _server;

        private string _userName;

        //private string _userPassword;
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            _client = tcpClient;
            _server = serverObject;
            serverObject.AddConnection(this);
        }

        protected internal string Id { get; }
        protected internal NetworkStream Stream { get; private set; }

        /// <summary>
        ///     Main client function, sends message to other clients and prints broadcasted messages
        /// </summary>
        public void Process()
        {
            try
            {
                Stream = _client.GetStream();
                var message = GetMessage();
                _userName = message;
                //_userPassword = message.Split(' ')[1];

                message = _userName + " Entered the chat with ID:" + Id;
                _server.BroadcastMessage(message, Id);
                //_server.BroadcastMessageToServer(_userPassword, Id);
                Console.WriteLine(message);
                while (true)
                    try
                    {
                        message = GetMessage();
                        message = $"{_userName}: {message}";
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message, Id);
                    }
                    catch
                    {
                        message = $"{_userName}: Left chat";
                        Console.WriteLine(message);
                        _server.BroadcastMessage(message, Id);
                        break;
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        /// <summary>
        ///     Gets message from network stream
        /// </summary>
        /// <returns>String representation of byte array</returns>
        private string GetMessage()
        {
            var data = new byte[64];
            var builder = new StringBuilder();
            do
            {
                var bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            } while (Stream.DataAvailable);

            return builder.ToString();
        }

        /// <summary>
        ///     Close connection for this client
        /// </summary>
        protected internal void Close()
        {
            Stream?.Close();
            _client?.Close();
        }
    }
}