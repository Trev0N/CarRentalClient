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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.IO;

namespace CarRentalClient

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char NewChar = (char)32;
        private String Token;

        public MainWindow(string Token)
        {
            this.Token = Token;
        }

        public MainWindow()
        {
            InitializeComponent();
            TextBlockFormatting();
        }


        public void TextBlockFormatting()
        {
            CenterWindowOnScreen();
            List<Car> car;
            car = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            myDataGrid.ItemsSource = car;
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




       
       //ADD GARAGE TAB
        private void InitializeAddGarageTab()
        {
            List<GarageList> garageList;
            GetRequest("http://localhost:8080/garage/", Token);
            garageList =JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));


            addGarageDataGrid.ItemsSource=garageList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String Name = addGarageName.Text;
            String Address = addGarageAddress.Text;

            List<GarageList> garageList;
            GetRequest("http://localhost:8080/garage/", Token);
            garageList = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));

            

            if (Name != null && Address != null && Name != "" && Address !="")
            {
                                             
                String json = "{" +
                                 "\"address\": \"" + Address + "\"," +
                                 "\"name\": \""+Name+"\""
                                + "}";

                foreach (GarageList garage in garageList)
                {

                    if (addGarageDataGrid.SelectedItem != null && addGarageDataGrid.SelectedItem.ToString().Equals(garage.ToString()))
                    {
                        PutRequest("http://localhost:8080/garage/edit/" + garage.ID, Token, json);
                        break;
                    }
                    else
                    {
                        System.Windows.MessageBoxResult message = System.Windows.MessageBox.Show("Are you sure that you want create new garage?","Create garage confirmation", System.Windows.MessageBoxButton.YesNo);
                        if (message == MessageBoxResult.Yes)
                        {
                            PostRequest("http://localhost:8080/garage/create", Token, json);
                            break;
                        }
                        else
                            break;
                    }
                }
                InitializeAddGarageTab();
            }
            else
                MessageBox.Show("You have to complete all fields. ");
        }

        //END

        private void Button_Create_Car(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Edit_Car(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Delete_Car(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Delete_Garage(object sender, RoutedEventArgs e)
        {
            deleteGarageTab.IsEnabled = true;
            deleteGarageTab.IsSelected = true;
            List<GarageList> garageLists = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));


            deleteGarageComboBox.ItemsSource = garageLists;
        }



        private void Button_Add_Garage(object sender, RoutedEventArgs e)
        {
            addGarageTab.IsEnabled = true;
            addGarageTab.IsSelected = true;
            InitializeAddGarageTab();
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


        static string GetRequest(string url, string Token)
        {

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
                var response = client.GetAsync(url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }


        }

        private void PostRequest(String url, String token, String json)
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


        private void PutRequest(String url, String token, String json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "PUT";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + Token);
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))

            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            }
            catch(WebException e)
            {
                MessageBox.Show("Garage name isn't available, please change it!");
            }
            }

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



        private void AddGarageDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text=null;
            if (addGarageDataGrid.SelectedItem != null)
            {
                text = addGarageDataGrid.SelectedItem.ToString();
            }
            List<GarageList> garageList;
            garageList = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));
            foreach(GarageList garage in garageList)
            {
                if (garage.ToString().Equals(text))
                {
                    addGarageName.Text = garage.Name;
                    addGarageAddress.Text = garage.Address;
                    break;
                }

            }
        }

        private void Delete_Garage_Click(object sender, RoutedEventArgs e)
        {
            String garage = deleteGarageComboBox.SelectedItem.ToString();
            Console.WriteLine(garage);
        }
    }
}
