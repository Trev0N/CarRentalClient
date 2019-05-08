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
            List<CarReadyToRent> carReadyToRents;
            carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent("http://localhost:8080/car/readytorent", Token));
            myDataGrid.ItemsSource = carReadyToRents;
            if (carReadyToRents.Count == 0)
            {
                rentCar.IsEnabled = false;
            }
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

      
       

        private void PostRentCar(String url, String token,String json)
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
        static string DeleteRent(string url, string Token)
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

        private void ReturnMainMenu_Click(object sender, RoutedEventArgs e)
        {
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
                if(rentedCar.Mark + " " + rentedCar.Model + " " + rentedCar.RegisterName == car)
                {
                    id = rentedCar.IDRent;
                    DeleteRent("http://localhost:8080/rent/return/" + id, Token);
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
    }
}
