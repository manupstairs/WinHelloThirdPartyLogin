using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace WinHelloThirdPartyLogin
{
    

    public class UWPSocketServer
    {
        public StreamSocketListener StreamSocketListener { get; set; }

        private StreamSocket ServerSocket { get; set; }

        public async void StartServer()
        {
            try
            {
                StreamSocketListener = new StreamSocketListener();
                
                // The ConnectionReceived event is raised when connections are received.
                StreamSocketListener.ConnectionReceived += this.StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await StreamSocketListener.BindServiceNameAsync("23333");

                //this.serverListBox.Items.Add("server is listening...");
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
                //this.serverListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        private async void StreamSocketListener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            ServerSocket = args.Socket;


            //DataWriter writer = new DataWriter(ServerSocket.OutputStream);
            //writer.WriteString(request);

            //await writer.StoreAsync();

            //DataReader reader = new DataReader(args.Socket.InputStream);

            //while (true)
            //{
                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                {
                    var request = await streamReader.ReadLineAsync();
                }
            //}

           
        }

        public async void Send(string message)
        {
            using (Stream outputStream = ServerSocket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(message);
                    await streamWriter.FlushAsync();
                }
            }
        }
    }

    
}
