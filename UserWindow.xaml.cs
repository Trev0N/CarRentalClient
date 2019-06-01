using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace CarRentalClient
{
    /// <summary>
    /// Logika interakcji dla klasy UserPanel.xaml
    /// </summary>
    public partial class UserPanel : Window
    {
        private String Token;
        private const String Address = "https://carrental-wsiz.herokuapp.com/";
        /// <summary>
        /// Konstruktor klasy z tokenem aby przekazać z ekranu logowania token do tej klasy
        /// </summary>
        /// <param name="token"></param>
        public UserPanel(string token)
        {
            Token = token;
        }
        /// <summary>
        /// Pusty konstruktor który iniaclizuje potrzebne komponenety okienka
        /// </summary>
        public UserPanel()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Metoda która wypełnia datagrid
        /// </summary>
        public void InitializeDataGrid()
        {
            CenterWindowOnScreen();
            List<CarReadyToRent> carReadyToRents;
            carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent(Address+"car/readytorent", Token));
            myDataGrid.ItemsSource = carReadyToRents;
            if (carReadyToRents.Count == 0)
                rentCar.IsEnabled = false;
            else
                rentCar.IsEnabled = true;
        }
        /// <summary>
        /// Metoda centrująca okienko na środku ekranu
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
        /// Przełącza nam do Rent Car Tab i uzupełnia całe okienko potrzebnymi danymi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Rent_Car(object sender, RoutedEventArgs e)
        {
   
            rentTab.IsEnabled = true;
            rentTab.IsSelected = true;


            List<CarReadyToRent> carReadyToRents;
            carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent(Address+"car/readytorent", Token));

            foreach (CarReadyToRent carReadyToRent in carReadyToRents)
            {
                listCar.Items.Add(carReadyToRent.Mark + " " + carReadyToRent.Model);
            }

            
        }
        /// <summary>
        /// Wysyła Request do serwera o wypożyczenie samochodu
        /// </summary>
        private void Button_Rent(object sender, RoutedEventArgs e)
        {
            if (dateRent.SelectedDate != null && listCar.SelectedItem != null)
            {



                int day = dateRent.SelectedDate.Value.Day;
                int month = dateRent.SelectedDate.Value.Month;
                int year = dateRent.SelectedDate.Value.Year;
                DateTime dateTime = dateRent.SelectedDate.Value.ToUniversalTime();
                String car = listCar.SelectedItem.ToString();


                List<CarReadyToRent> carReadyToRents;
                carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent(Address+"car/readytorent", Token));
                long id;
                String json = "{" +
                    "\"rentEndDate\": \"" + dateTime.ToString("yyyy-MM-dd") + "T00:00:00.000Z" + "\"" +
                    "}";
                if (dateTime < DateTimeOffset.Now)
                {
                    MessageBox.Show("Choose correct data");
                    listCar.Items.Clear();
                    listCar.Items.Refresh();
                }
                else
                    foreach (CarReadyToRent carReadyToRent in carReadyToRents)
                    {
                        if (carReadyToRent.Mark + " " + carReadyToRent.Model == car)
                        {
                            id = carReadyToRent.ID;
                            PostRentCar(Address+"rent/car/" + id, Token, json);
                            listCar.Items.Clear();
                            listCar.Items.Refresh();
                            InitializeDataGrid();
                            break;

                        }
                    }

            }
            else
            {
                MessageBox.Show("You have to choose car and correct date.");
                listCar.Items.Clear();
                listCar.Items.Refresh();
            }

            InitializeDataGrid();
            mainMenu.IsSelected = true;
            rentTab.IsEnabled = false;


        }

        /// <summary>
        /// Przełącza nasze okno na widok naszych wypożyczeń w serwisie
        /// </summary>
        private void Button_Get_Rents(object sender, RoutedEventArgs e)
        {
            allRentedCars.IsEnabled = true;
            allRentedCars.IsSelected = true;
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent(Address+"rent/", Token));
            dataGridRentings.ItemsSource = rentedCars;
        }


        /// <summary>
        /// Obsługa zaznaczenia , po zaznaczeniu filtrujemy datagrid
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent(Address+"rent/", Token));
            List<RentedCars> actualRentedCars = new List<RentedCars>();
            if(rentedCars!=null)
            foreach(RentedCars rented in rentedCars)
            {
                if(rented.RentEndDate > DateTimeOffset.Now)
                {
                    if(rented!=null)
                        actualRentedCars.Add(rented);
                }
            }

            dataGridRentings.ItemsSource = actualRentedCars;
        }
        /// <summary>
        /// Odkliknięcie wyłącza filtr i spowrotem pokazuje wszystkie wypożyczenia 
        /// </summary>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent(Address+"rent/", Token));
            dataGridRentings.ItemsSource = rentedCars;
        }
        /// <summary>
        /// Przełącza do okienka z oddawaniem samochodu
        /// </summary>
        private void Button_Return_Car(object sender, RoutedEventArgs e)
        {
            returnCarTab.IsEnabled = true;
            returnCarTab.IsSelected = true;
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent(Address+"rent/", Token));
            if(rentedCars!=null)
            foreach(RentedCars rentedCar in rentedCars)
            {
                if(rentedCar.RentEndDate>DateTimeOffset.Now)
                comboBoxReturnCar.Items.Add(rentedCar.Mark + " " + rentedCar.Model + " " + rentedCar.RegisterName);
            }

        }
        /// <summary>
        /// Powrót do głównego menu
        /// </summary>
        private void ReturnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            checkBoxAllRentings.IsChecked=false;
            mainMenu.IsSelected = true;
            allRentedCars.IsEnabled = false;
            InitializeDataGrid();
        }
        /// <summary>
        /// Zwraca samochód poprzez request DELETE do serwera
        /// </summary>
        private void ButtonReturnCar_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxReturnCar.SelectedItem != null)
            {
                String car = comboBoxReturnCar.SelectedItem.ToString();
           
            long id;
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent(Address+"rent/", Token));
            foreach (RentedCars rentedCar in rentedCars)
            {
                if(rentedCar.Mark + " " + rentedCar.Model + " " + rentedCar.RegisterName == car&&rentedCar.RentEndDate>DateTimeOffset.Now)
                {
                    id = rentedCar.IDRent;
                    DeleteRequest(Address+"rent/return/" + id, Token);
                    mainMenu.IsSelected = true;
                    comboBoxReturnCar.Items.Clear();
                    returnCarTab.IsEnabled = false;
                    break;
                }
            }
                InitializeDataGrid();
            }
            else
                MessageBox.Show("You have to select some car");
        }
        /// <summary>
        /// Wylogowuje z serwisu
        /// </summary>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequest(Address+"sign-out", Token);
            LoginScreen login = new LoginScreen();
            login.InitializeComponent();
            login.Show();
            this.Close();
        }




        //REQUESTS
        /// <summary>
        /// DELETE request
        /// </summary>
        /// <param name="url">adres serwera </param>
        /// <param name="Token">token z logowania</param>
        /// <returns></returns>
        static string DeleteRequest(string url, string Token)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.DeleteAsync(url).Result;
                String responseee = response.Content.ReadAsStringAsync().Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        /// <summary>
        /// GET request
        /// </summary>
        /// <param name="url">adres serwera</param>
        /// <param name="Token">token z logowania</param>
        /// <returns></returns>
        static string GetCarsReadyToRent(string url, string Token)
        {

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.GetAsync(url).Result;

                String responseee = response.Content.ReadAsStringAsync().Result;
                return response.Content.ReadAsStringAsync().Result;
            }


        }
        /// <summary>
        /// Post Request
        /// </summary>
        /// <param name="url">adres serwera</param>
        /// <param name="token">token z logowania</param>
        /// <param name="json">dane w JSON</param>
        private void PostRentCar(String url, String token, String json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + Token);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

        }
    }
}
