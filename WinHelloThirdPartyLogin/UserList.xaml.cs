using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Credentials.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinHelloThirdPartyLogin.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinHelloThirdPartyLogin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserList : Page
    {
        private SocketClient SocketClient { get; set; }
        //private SocketServer SocketServer { get; set; }

        public UserList()
        {
            this.InitializeComponent();
            Loaded += UserList_Loaded;

            SocketClient = new SocketClient();
            SocketClient.ReceiveMessageEvent += SocketClient_ReceiveMessageEvent;
            SocketClient.StartConnectAsync();

            //SocketServer = new SocketServer();
            //SocketServer.ReceiveMessageEvent += SocketServer_ReceiveMessageEvent;
            //SocketServer.StartListen();
        }

        private async void SocketClient_ReceiveMessageEvent(object sender, string e)
        {
            var account = AccountHelper.AccountList.FirstOrDefault(a => a.Username == e);
            if (account != null)
            {
                var consentResult = await UserConsentVerifier.RequestVerificationAsync(account.Username);
                if (consentResult == UserConsentVerificationResult.Verified)
                {
                    SocketClient.Send("Verified");
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { Frame.Navigate(typeof(Welcome), account); });
                    
                }
            }
        }

        //private async void SocketServer_ReceiveMessageEvent(object sender, string e)
        //{
        //    var account = AccountHelper.AccountList.FirstOrDefault(a => a.Username == e);
        //    if (account != null)
        //    {
        //        var consentResult = await UserConsentVerifier.RequestVerificationAsync(account.Username);
        //        if (consentResult == UserConsentVerificationResult.Verified)
        //        {
        //            SocketServer.Send("Verified");
        //        }
        //        //else
        //        //{

        //        //}
        //    }
        //}

        private async void UserList_Loaded(object sender, RoutedEventArgs e)
        {
            await AccountHelper.LoadAccountListAsync();
            if (AccountHelper.AccountList.Count == 0)
            {
                //If there are no accounts navigate to the LoginPage
                Frame.Navigate(typeof(PassportRegister));
            }
            UserListView.ItemsSource = AccountHelper.AccountList;
            UserListView.SelectionChanged += UserSelectionChanged;
        }

        private void UserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListView)sender).SelectedValue != null)
            {
                Account account = (Account)((ListView)sender).SelectedValue;
                if (account != null)
                {
                    SocketClient.Send(account.Username);
                    
                }
                else
                { 
                    Frame.Navigate(typeof(PassportRegister));
                }
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PassportRegister));
        }
    }
}
