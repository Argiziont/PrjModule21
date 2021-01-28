using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPIPLib
{
    public class ServerObject
    {
        private static TcpListener _tcpListener;
        private readonly List<ClientObject> _clients = new();

        /// <summary>
        ///     Add new connection
        /// </summary>
        /// <param name="clientObject">Client to work with</param>
        public void AddConnection(ClientObject clientObject)
        {
            _clients.Add(clientObject);
        }

        /// <summary>
        ///     Delete client from connection
        /// </summary>
        /// <param name="id">Client ID</param>
        public void RemoveConnection(string id)
        {
            var client = _clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                _clients.Remove(client);
        }

        /// <summary>
        ///     Server listener which checks if someone send message
        /// </summary>
        public void Listen()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8888);
                _tcpListener.Start();
                Console.WriteLine("Server is running. Waiting for connections...");

                while (true)
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();

                    var clientObject = new ClientObject(tcpClient, this);
                    var clientThread = new Thread(clientObject.Process);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        /// <summary>
        ///     Broadcast message to all clients excluding sender
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="id">Id of sender to exclude it from broadcast</param>
        public void BroadcastMessage(string message, string id)
        {
            var data = Encoding.Unicode.GetBytes(message);
            foreach (var t in _clients.Where(t => t.Id != id))
                t.Stream.Write(data, 0, data.Length);
        }

        //public void BroadcastMessageToServer(string message, string id)
        //{
        //    //var data = Encoding.Unicode.GetBytes(message);
        //    if (message.Length < 6)
        //        throw new ArgumentException("Password is too week, minimum 6 characters needed");

        //    Console.WriteLine(message);
        //        //Save confident data
        //}

        /// <summary>
        ///     Close connection for each client end stop server
        /// </summary>
        public void Disconnect()
        {
            _tcpListener.Stop();

            foreach (var t in _clients)
                t.Close();

            Environment.Exit(0);
        }
    }
}