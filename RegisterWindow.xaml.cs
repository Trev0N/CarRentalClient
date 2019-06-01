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
    {/// <summary>
     /// Pusty konstruktor centrujący okno i iniaclizujący okienko
     /// </summary>
        private const String Address = "https://carrental-wsiz.herokuapp.com/";
        public RegisterScreen()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }
        /// <summary>
        /// Metoda centrująca okienko na ekranie
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        /// <summary>
        /// Request POST do serwera z danymi nowego użytkownika w formacie JSON
        /// </summary>
        /// <param name="url">adres serwera</param>
        /// <param name="Json">dane w JSON</param>
        /// <returns></returns>
        public static Boolean PostRegister(string url, string Json)
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
        /// <summary>
        /// Metoda przygotująca dane do serwera i wywołująca zapytanie
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (firstName.Text != "" && lastName.Text != "" && login.Text != "" && mail.Text != "" && password.Password != "")
            {
                String Json =
       "{  \"firstName\": \"" + firstName.Text + "\","
     + " \"lastName\": \"" + lastName.Text + "\","
     + " \"login\": \"" + login.Text + "\","
     + " \"mail\": \"" + mail.Text + "\","
     + " \"password\": \"" + password.Password + "\","
     + " \"recaptcha\": \"ABCDEFGH\""
     + "}";

                if (PostRegister(Address + "user/sign-in", Json).Equals(true))
                {

                    LoginScreen loginScreen = new LoginScreen();
                    loginScreen.InitializeComponent();
                    loginScreen.txtUsername = login;
                    loginScreen.txtPassword = password;
                    loginScreen.Login();
                    this.Close();
                }
            }
            else
                MessageBox.Show("You have to complete all fields");
        }
        /// <summary>
        /// Obsługa guzika powrotu do ekranu logowania
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.InitializeComponent();
            loginScreen.Show();
            this.Close();
        }
    }
}
