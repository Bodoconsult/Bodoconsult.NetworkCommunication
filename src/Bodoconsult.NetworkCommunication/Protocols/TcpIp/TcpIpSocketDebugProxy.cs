// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using Bodoconsult EDV-Dienstleistungen GmbH.StSys.SQL.Model.Logging;
//using Bodoconsult EDV-Dienstleistungen GmbH.StSys.deviceCommunication.Interfaces;

//namespace Bodoconsult EDV-Dienstleistungen GmbH.StSys.deviceCommunication.Transport.TcpIp
//{
//    /// <summary>
//    /// Current implementation of <see cref="ISocketProxy"/>
//    /// </summary>
//    public class TcpIpSocketDebugProxy : BaseTcpIpSocketProxy
//    {

//        private readonly IAppLoggerProxy _log;

//        private readonly string _socketId;

//        private static readonly Random _random = new Random();



//        /// <summary>
//        /// Default ctor
//        /// </summary>
//        public TcpIpSocketDebugProxy(IAppLoggerProxy logger)
//        {
//            _socketId = $"Socket{RandomString(10)}";
//            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
//            {
//                ReceiveTimeout = ReceiveTimeout,
//                SendTimeout = SendTimeout
//            };

//            _log = logger;

//            Log("Init debug socket");
//        }


//        /// <summary>
//        /// Generate a random string for socket name
//        /// </summary>
//        /// <param name="length">Length of the random name</param>
//        /// <returns>Random name of the socket</returns>
//        public static string RandomString(int length)
//        {
//            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//            return new string(Enumerable.Repeat(chars, length)
//                .Select(s => s[_random.Next(s.Length)]).ToArray());
//        }


//        /// <summary>
//        /// Is the socket connected
//        /// </summary>
//        public override bool Connected
//        {
//            get
//            {
//                var connected = Socket.Connected;
//                Log($"Connected: {connected}");
//                return Socket.Connected;
//            }
//        }

//        /// <summary>
//        /// Are Bytes available to read
//        /// </summary>
//        public override int BytesAvailable
//        {
//            get
//            {
//                var available = Socket.Available;
//                Log($"BytesAvailable: {available}");
//                return available;
//            }
//        }

//        /// <summary>
//        /// Send bytes
//        /// </summary>
//        /// <param name="bytesToSend">Byte array to send</param>
//        public override Task<int> Send(byte[] bytesToSend)
//        {
//            Log("Send", bytesToSend);
//            return Socket.SendAsync(bytesToSend);
//        }



//        /// <summary>
//        /// Shut the socket down
//        /// </summary>
//        public override void Shutdown()
//        {
//            Log("Shutdown");
//            Socket.Shutdown(SocketShutdown.Both);
//        }

//        /// <summary>
//        /// Close the socket
//        /// </summary>
//        public override void Close()
//        {
//            Log("Close");
//            Socket.Close();
//        }

//        /// <summary>
//        /// Connect to an IP endpoint
//        /// </summary>
//        /// <param name="endpoint">IP endpoint</param>
//        public override async Task Connect(IPEndPoint endpoint)
//        {
//            await Task.Run(() =>
//            {
//                Log($"Connect: {endpoint.Address} port {endpoint.Port}");
//                Socket.Connect(endpoint);
//            });

//        }


//        /// <summary>
//        /// Bind to an IP endpoint
//        /// </summary>
//        /// <param name="endpoint">IP endpoint</param>
//        public override void Bind(IPEndPoint endpoint)
//        {
//            Log($"Bind: {endpoint.Address} port {endpoint.Port}");
//            Socket.Bind(endpoint);
//        }


//        /// <summary>
//        /// Listen
//        /// </summary>
//        /// <param name="backlog">The maximum length of pending messages queue</param>
//        public override void Listen(int backlog)
//        {
//            Socket.Listen(backlog);
//        }

//        /// <summary>
//        /// Receive data from the socket
//        /// </summary>
//        /// <param name="buffer">Byte array to store the received byte data in</param>
//        /// <returns>Number of bytes received</returns>
//        public override Task<int> Receive(byte[] buffer)
//        {
//            var length = Socket.ReceiveAsync(buffer);
//            Log("Receive", buffer);
//            return length;
//        }

//        /// <summary>
//        /// Receive data from the socket
//        /// </summary>
//        /// <param name="buffer">Byte array to store the received byte data in</param>
//        /// <param name="offset">Offset</param>
//        /// <param name="expectedBytesLength">Expected length of the byte data received</param>
//        /// <returns>Number of bytes received</returns>
//        public override Task<int> Receive(byte[] buffer, int offset, int expectedBytesLength)
//        {
//            var i = Task.Run(() => Socket.Receive(buffer, offset, expectedBytesLength, SocketFlags.None));
//            Log($"Receive({offset}, {expectedBytesLength})", buffer);
//            return i;
//        }

//        /// <summary>
//        /// Send bytes 
//        /// </summary>
//        /// <param name="bytesToSend">Byte array to send</param>
//        /// <param name="offset">Offset</param>
//        /// <param name="messageBytesLength">Number of message bytes length to send</param>
//        /// <returns></returns>
//        public override Task<int> Send(byte[] bytesToSend, int offset, int messageBytesLength)
//        {
//            Log("Send", bytesToSend);
//            return Socket.SendAsync(bytesToSend, offset, messageBytesLength, SocketFlags.None);
//        }

//        /// <summary>
//        /// Poll data
//        /// </summary>
//        /// <returns>True, if data can be read, else false</returns>
//        public override bool Poll()
//        {
//            var result = Socket.Poll(PollingTimeout, SelectMode.SelectRead);
//            Log($"Poll{PollingTimeout}: result {result}");
//            return result;
//        }

//        /// <summary>
//        /// Send a file
//        /// </summary>
//        /// <param name="fileName">Full file path</param>
//        public override void SendFile(string fileName)
//        {
//            Log($"SendFile:{fileName}");
//            Socket.SendFile(fileName);
//        }

//        /// <summary>
//        /// Current socket (only for testing purposes, do not access directly in production code)
//        /// </summary>
//        public Socket Socket { get; }




//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        public override void Dispose()
//        {
//            Log("Dispose");
//            Socket?.Dispose();
//        }

//        /// <summary>
//        /// Log a information message
//        /// </summary>
//        /// <param name="msg"></param>
//        private void Log(string msg)
//        {
//            //Debug.Print($"{_socketId}:{msg}");
//            _log.LogInformation($"{_socketId}:{msg}");
//        }


//        /// <summary>
//        /// Log a byte array
//        /// </summary>
//        /// <param name="title">Title</param>
//        /// <param name="data">Byte data to log</param>
//        private void Log(string title, byte[] data)
//        {
//            var d = new byte[data.Length];

//            Buffer.BlockCopy(data, 0, d, 0, data.Length);

//            var s = new StringBuilder();
//            s.Append($"{_socketId}:{title}: ");
//            foreach (var x in d)
//            {
//                s.Append($"[{x}]");
//            }

//            var msg = s.ToString();
//            //Debug.Print(msg);
//            _log.LogInformation(msg);
//        }
//    }
//}
