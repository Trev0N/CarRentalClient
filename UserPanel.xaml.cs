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




        private void Button_Get_Rents(object sender, RoutedEventArgs e)
        {
            List<RentedCars> rentedCars;
            rentedCars = JsonConvert.DeserializeObject<List<RentedCars>>(GetCarsReadyToRent("http://localhost:8080/rent/", Token));
            dataGridRentings.ItemsSource = rentedCars;
        }

        private void Button_Return_Car(object sender, RoutedEventArgs e)
        {

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
                "\"rentEndDate\": \""+dateTime.ToString("yyyy-MM-dd")+ "T00:00:00.000Z" + "\"" +
                "}";

            foreach(CarReadyToRent carReadyToRent in carReadyToRents)
            {
                if(carReadyToRent.Mark +" "+ carReadyToRent.Model == car)
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
    }
}
