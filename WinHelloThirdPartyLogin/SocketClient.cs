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
    class SocketClient
    {
        private IAsyncAction ReciveAsyncAction { get; set; }
        private Socket Client { get; set; }
        public event EventHandler<string> ReceiveMessageEvent;

        public SocketClient()
        {
            
        }

        public async void StartConnectAsync()
        {
            Debug.WriteLine("Hello World! I'm client!");
            //创建实例
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = "192.168.11.130";
            var port = 2333;
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPEndPoint point = new IPEndPoint(ip, port);
            //进行连接
            Client.Connect(point);

            ReciveAsyncAction = ThreadPool.RunAsync(
                workItem =>
                {
                    Recive();
                });
            await ReciveAsyncAction;
        }


        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="o"></param>
        private void Recive()
        {
            while (true)
            {
                //获取发送过来的消息
                byte[] buffer = new byte[1024 * 1024 * 2];
                var effective = Client.Receive(buffer);
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

            byte[] byteArray = Encoding.UTF8.GetBytes(content.ToCharArray());
            Client.Send(byteArray);
        }
    }
}
