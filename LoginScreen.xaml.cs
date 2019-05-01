using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarRentalClient
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    /// 
    

    public partial class LoginScreen : Window
    {
        
        static string GetToken(string url, string userName, string password)
        {
            
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("scope","ui"),
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "password", password )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Basic YnJvd3Nlcjo=");
                var response = client.PostAsync(url, content).Result;
                          
                return response.Content.ReadAsStringAsync().Result;
            }
            

        }
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            String response = GetToken("http://localhost:8080/api/login", txtUsername.Text, txtPassword.Password);
            String token;
            if (response.Contains("access_token"))
            {
                token = response.Substring(17, 36);
                MainWindow mainWindow = new MainWindow(token);
                mainWindow.Show();
                mainWindow.InitializeComponent();
                mainWindow.TextBlockFormatting();
                this.Close();
            }
            else
                MessageBox.Show("Wrong login or password");
            
        }
    }
}
