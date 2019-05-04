using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        }

        private void Button_Return_Car(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Rent(object sender, RoutedEventArgs e)
        {

            String data = dateRent.SelectedDate.Value.ToString();
            String car = listCar.SelectedItem.ToString();
            List<CarReadyToRent> carReadyToRents;
            carReadyToRents = JsonConvert.DeserializeObject<List<CarReadyToRent>>(GetCarsReadyToRent("http://localhost:8080/car/readytorent", Token));
            long id;
            foreach(CarReadyToRent carReadyToRent in carReadyToRents)
            {
                if(carReadyToRent.Mark +" "+ carReadyToRent.Model == car)
                {
                    id = carReadyToRent.ID;
                    PostRentCar("http://localhost:8080/rent/car/" + id, Token, data);
                    break;

                }
            }

            

        }


        private String PostRentCar(String url, String token,String date)
        {

            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "rentEndDate", date )

                    };
            
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("ContentType", "application/json");                
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.PostAsync(url, content);


                return response.AsyncState.ToString();
            }
        }
    }
}
