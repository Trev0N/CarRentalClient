using System;
using System.Collections.Generic;
using System.IO;
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
    /// Logika interakcji dla klasy RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : Window
    {
        public RegisterScreen()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        static Boolean PostRegister(string url, string Json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            try
            {
                using (
                    var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(Json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }
                return true;
            }catch(Exception)
            {
                MessageBox.Show("Check your internet connection, or server error");
                return false;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String Json = 
   "{  \"firstName\": \""+firstName.Text+"\","
 + " \"lastName\": \""+lastName.Text+"\","
 + " \"login\": \""+login.Text+"\","
 + " \"mail\": \""+mail.Text+"\","
 + " \"password\": \""+password.Password+"\","
 + " \"recaptcha\": \"ABCDEFGH\""
 +"}";

            if (PostRegister("http://localhost:8080/user/sign-in", Json).Equals(true))
            {

                LoginScreen loginScreen = new LoginScreen();
                loginScreen.InitializeComponent();
                loginScreen.txtUsername = login;
                loginScreen.txtPassword = password;
                loginScreen.login();
                this.Close();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.InitializeComponent();
            loginScreen.Show();
            this.Close();
        }
    }
}
