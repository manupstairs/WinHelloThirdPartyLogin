using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThirdPartyLoginServer
{
    class SocketServer
    {
        public event EventHandler<string> ReceiveMessageEvent;

        private Socket ServerSocket { get; set; }

        private EndPoint LoginEndPoint { get; set; }

        public SocketServer()
        {
            
        }

        public void StartListen()
        {
            Console.WriteLine("Hello World!");
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Any;
            IPEndPoint point = new IPEndPoint(ip, 23333);
            //socket绑定监听地址
            ServerSocket.Bind(point);
            Console.WriteLine("Listen Success");
            //设置同时连接个数
            ServerSocket.Listen(10);

            //利用线程后台执行监听,否则程序会假死
            Thread thread = new Thread(Listen);
            thread.IsBackground = true;
            thread.Start(ServerSocket);

            Console.Read();
        }

        /// <summary>
        /// 监听连接
        /// </summary>
        /// <param name="o"></param>
        private void Listen(object o)
        {
            var serverSocket = o as Socket;
            while (true)
            {
                //等待连接并且创建一个负责通讯的socket
                var send = serverSocket.Accept();
                //获取链接的IP地址
                var sendIpoint = send.RemoteEndPoint.ToString();
                Console.WriteLine($"{sendIpoint}Connection");
                //开启一个新线程不停接收消息
                Thread thread = new Thread(Recive);
                thread.IsBackground = true;
                thread.Start(send);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="o"></param>
        private void Recive(object o)
        {
            var send = o as Socket;
            while (true)
            {
                //获取发送过来的消息容器
                byte[] buffer = new byte[1024 * 1024 * 2];
                var effective = send.Receive(buffer);
                //有效字节为0则跳过
                if (effective == 0)
                {
                    break;
                }
                var str = Encoding.UTF8.GetString(buffer, 0, effective);
                
                ReceiveMessageEvent?.Invoke(this, str);

                if (string.Compare(str, "loginclient", true) == 0)
                {
                    LoginEndPoint = send.RemoteEndPoint;
                }
                else if (LoginEndPoint != null && LoginEndPoint != send.RemoteEndPoint)
                {
                    SendMessage(send, str);
                }
            }
        }

        private void SendMessage(Socket sender, string message)
        {
            byte[] bytes = new byte[message.Length * sizeof(char)];
            Buffer.BlockCopy(message.ToCharArray(), 0, bytes, 0, bytes.Length);
            //sender.Send(bytes);
            sender.SendTo(bytes, LoginEndPoint);
            Console.WriteLine(message);
        }
    }
}
