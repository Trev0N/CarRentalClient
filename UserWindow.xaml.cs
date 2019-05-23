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

        public UserPanel(string token)
        {
            Token = token;
        }

        public UserPanel()
        {
            InitializeComponent();
        }

        public void InitializeDataGrid()
        {
            CenterWindowOnScreen();
            List<CarReadyToRent> carReadyToRents;
            carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent("http://localhost:8080/car/readytorent", Token));
            myDataGrid.ItemsSource = carReadyToRents;
            if (carReadyToRents.Count == 0)
                rentCar.IsEnabled = false;
            else
                rentCar.IsEnabled = true;
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


        private void Button_Rent_Car(object sender, RoutedEventArgs e)
        {
   
            rentTab.IsEnabled = true;
            rentTab.IsSelected = true;


            List<CarReadyToRent> carReadyToRents;
            carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent("http://localhost:8080/car/readytorent", Token));

            foreach (CarReadyToRent carReadyToRent in carReadyToRents)
            {
                listCar.Items.Add(carReadyToRent.Mark + " " + carReadyToRent.Model);
            }

            
        }

        private void Button_Rent(object sender, RoutedEventArgs e)
        {
            if (dateRent.SelectedDate != null || listCar.SelectedItem != null)
            {



                int day = dateRent.SelectedDate.Value.Day;
                int month = dateRent.SelectedDate.Value.Month;
                int year = dateRent.SelectedDate.Value.Year;
                DateTime dateTime = dateRent.SelectedDate.Value.ToUniversalTime();
                String car = listCar.SelectedItem.ToString();


                List<CarReadyToRent> carReadyToRents;
                carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent("http://localhost:8080/car/readytorent", Token));
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
                            PostRentCar("http://localhost:8080/rent/car/" + id, Token, json);
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


        private void Button_Get_Rents(object sender, RoutedEventArgs e)
        {
            allRentedCars.IsEnabled = true;
            allRentedCars.IsSelected = true;
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent("http://localhost:8080/rent/", Token));
            dataGridRentings.ItemsSource = rentedCars;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent("http://localhost:8080/rent/", Token));
            List<RentedCars> actualRentedCars = new List<RentedCars>();
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
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent("http://localhost:8080/rent/", Token));
            dataGridRentings.ItemsSource = rentedCars;
        }

        private void Button_Return_Car(object sender, RoutedEventArgs e)
        {
            returnCarTab.IsEnabled = true;
            returnCarTab.IsSelected = true;
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent("http://localhost:8080/rent/", Token));
            foreach(RentedCars rentedCar in rentedCars)
            {
                if(rentedCar.RentEndDate>DateTimeOffset.Now)
                comboBoxReturnCar.Items.Add(rentedCar.Mark + " " + rentedCar.Model + " " + rentedCar.RegisterName);
            }

        }
        private void ReturnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            checkBoxAllRentings.IsChecked=false;
            mainMenu.IsSelected = true;
            allRentedCars.IsEnabled = false;
            InitializeDataGrid();
        }

        private void ButtonReturnCar_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxReturnCar.SelectedItem != null)
            {
                String car = comboBoxReturnCar.SelectedItem.ToString();
           
            long id;
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent("http://localhost:8080/rent/", Token));
            foreach (RentedCars rentedCar in rentedCars)
            {
                if(rentedCar.Mark + " " + rentedCar.Model + " " + rentedCar.RegisterName == car&&rentedCar.RentEndDate>DateTimeOffset.Now)
                {
                    id = rentedCar.IDRent;
                    DeleteRequest("http://localhost:8080/rent/return/" + id, Token);
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequest("http://localhost:8080/sign-out", Token);
            LoginScreen login = new LoginScreen();
            login.InitializeComponent();
            login.Show();
            this.Close();
        }




        //REQUESTS
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
