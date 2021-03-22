using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace broadcastTelnetServer {
    class Program {
        static void Main(string[] args) {
            Socket server = null;
            IPAddress addr = IPAddress.Parse("127.0.0.1");
            List<clientObj> clientsList = new List<clientObj>();

            try {
                server = new Socket(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(addr, 9999));
                server.Listen(100);
                while (true) {
                    clientObj client = new clientObj(server.Accept(), clientsList);
                    clientsList.Add(client);
                    Thread newThread = new Thread(client.startBroadcasting);
                    newThread.Start();
                }
            }
            catch {
                Console.WriteLine("Error acurred server can't run or can't get more connections.");
            }
        } // O(∞)
    }
}
