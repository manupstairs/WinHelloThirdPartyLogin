using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinHelloThirdPartyLogin.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinHelloThirdPartyLogin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PassportRegister : Page
    {
        private Account _account;
        public PassportRegister()
        {
            this.InitializeComponent();
        }

        private async void RegisterButton_Click_Async(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                //Register a new account
                _account = AccountHelper.AddAccount(WebsiteTextBox.Text,UsernameTextBox.Text,PasswordTextBox.Password);
                //Register new account with Microsoft Passport
                await MicrosoftPassportHelper.CreatePassportKeyAsync(_account.Username);
                //Navigate to the Welcome Screen. 
                Frame.Navigate(typeof(Welcome), _account);
            }
        }
    }
}
