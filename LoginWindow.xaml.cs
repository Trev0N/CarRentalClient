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
    /// Okno logowania
    /// Zawiera wszystkie potrzebne metody do zalogowania sie do serwisu
    /// </summary>
    public partial class LoginScreen : Window
    {

        private static String Address = "https://carrental-wsiz.herokuapp.com/";


        /// <summary>
        /// Pusty konstruktor klasy inicjalizujący potrzebne komponenty i centrujący okno na środku ekranu.
        /// </summary>
        public LoginScreen()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }


        /// <summary>
        /// Metoda która wysyła zapytanie do serwera aby uzyskać autoryzacje.
        /// </summary>
        /// <param name="url">Ścieżka do naszego serwera</param>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <param name="password">Hasło użytkownika</param>
        /// <returns>Zwraca string token</returns>
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
                try
                {
                    var response = client.PostAsync(url, content).Result;
                    return response.Content.ReadAsStringAsync().Result;
                } catch (Exception)
            {
                MessageBox.Show("Check internet connection, or server problem");
            }
                return null;
            }
            

        }
        /// <summary>
        /// Metoda centrująca okienka na środku ekranu 
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
        /// Metoda, która sprawdza czy dany uzytkownik ma role admina poprzez wysyłanie requesta
        /// </summary>
        /// <param name="url">adres serwera</param>
        /// <param name="Token">token z logowania</param>
        /// <returns>prawda jesli admin</returns>
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
        /// <summary>
        /// Obsługa guzika 
        /// </summary>
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Login();
            
        }
        /// <summary>
        /// Metoda logująca użytkownika do serwisu poprzez requesta
        /// </summary>
        public void Login()
        {
            String response = GetToken(Address + "api/login", txtUsername.Text, txtPassword.Password);
            String Token;
            if(response!=null)
            if (response.Contains("access_token"))
            {

                Token = response.Substring(17, 36);
               if (GetRole(Address+"user/isadmin", Token) == "true")
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
        /// <summary>
        /// Przełączenie do okienka rejestracji
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RegisterScreen registerScreen = new RegisterScreen();
            registerScreen.InitializeComponent();
            registerScreen.Show();
            this.Close();
        }
    }
}
