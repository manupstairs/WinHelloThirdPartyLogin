using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;

namespace WinHelloThirdPartyLogin
{
    class SocketServer
    {
        private IAsyncAction ListenAsyncAction { get; set; }
        private Socket ServerSide { get; set; }
        public event EventHandler<string> ReceiveMessageEvent;

        public SocketServer()
        {
            
        }

        public async void StartListen()
        {
            Debug.WriteLine("Hello World!");
            ServerSide = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Any;
            IPEndPoint point = new IPEndPoint(ip, 2333);
            //socket绑定监听地址
            ServerSide.Bind(point);
            Debug.WriteLine("Listen Success");
            //设置同时连接个数
            ServerSide.Listen(10);

            ListenAsyncAction = ThreadPool.RunAsync(
            (workItem) =>
            {
                Listen();
            });

            await ListenAsyncAction;
        }

        /// <summary>
        /// 监听连接
        /// </summary>
        /// <param name="o"></param>
        private async void Listen()
        {
            
            while (true)
            {
                //等待连接并且创建一个负责通讯的socket
                var send = ServerSide.Accept();
                //获取链接的IP地址
                var sendIpoint = send.RemoteEndPoint.ToString();
                Debug.WriteLine($"{sendIpoint}Connection");
                //开启一个新线程不停接收消息
                await ThreadPool.RunAsync(
            (workItem) =>
            {
                Recive(send);
            });
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
                Debug.WriteLine(str);
                ReceiveMessageEvent?.Invoke(this, str);
            }
        }

        public void Send(string content)
        {
            byte[] bytes = new byte[content.Length * sizeof(char)];
            Buffer.BlockCopy(content.ToCharArray(), 0, bytes, 0, bytes.Length);
            ServerSide.Send(bytes);
        }
    }
}
