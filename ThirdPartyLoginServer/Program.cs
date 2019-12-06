using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyLoginServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var SocketServer = new SocketServer();
            SocketServer.ReceiveMessageEvent += SocketServer_ReceiveMessageEvent;
            SocketServer.StartListen();

            Console.ReadKey();
        }

        private static void SocketServer_ReceiveMessageEvent(object sender, string e)
        {
            //Console.WriteLine(e);
        }
    }
}
