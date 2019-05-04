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
        static string GetRole(string url, string Token)
        {

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                HttpContent httpContent = null;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                var response = client.PostAsync(url, httpContent).Result;
                return response.Content.ReadAsStringAsync().Result;
            }


        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            String response = GetToken("http://localhost:8080/api/login", txtUsername.Text, txtPassword.Password);
            String Token;
            if (response.Contains("access_token"))
            {

                Token = response.Substring(17, 36);
                if (GetRole("http://localhost:8080/user/isadmin", Token) == "true")
                {
                    MainWindow mainWindow = new MainWindow(Token);
                    mainWindow.Show();
                    mainWindow.InitializeComponent();
                    mainWindow.TextBlockFormatting();
                    this.Close();
                }
                else
                {
                    UserPanel userPanel = new UserPanel(Token);
                    userPanel.Show();
                    userPanel.InitializeComponent();
                    userPanel.InitializeDataGrid();
                    this.Close();
                }
            }
            else
                MessageBox.Show("Wrong login or password");
            
        }

       
    }
}
