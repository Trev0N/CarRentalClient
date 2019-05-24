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
using CarRentalClient.UtilClasses;

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




       
       //ADD/EDIT GARAGE TAB
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
                if(garageList.Count>0)
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
                else
                    PostRequest("http://localhost:8080/garage/create", Token, json);
                InitializeAddGarageTab();
            }
            else
                MessageBox.Show("You have to complete all fields. ");
        }

        //END


            //Add/edit car

        public void InitializeAddCarTab()
        {
            List<Car> cars = null;
            if (GetRequest("http://localhost:8080/car/", Token).Contains("mark"))
            {
                 cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            }
            carDataGrid.ItemsSource = cars;

            List<GarageList> garageLists = null;
            if((GetRequest("http://localhost:8080/garage/", Token).Contains("name")));
                {
                garageLists = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));
            }
            garageComboBox.ItemsSource = garageLists;

        }


        private void Button_Create_Car(object sender, RoutedEventArgs e)
        {
            addCarTab.IsEnabled = true;
            addCarTab.IsSelected = true;
            InitializeAddCarTab();
        }

        private void Button_Edit_Car(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Delete_Car(object sender, RoutedEventArgs e)
        {
            deleteCarTab.IsEnabled = true;
            deleteCarTab.IsSelected = true;
            InitializeDeleteCarComboBox();
        }

        public void InitializeDeleteCarComboBox()
        {
            deleteCarComboBox.Items.Clear();
            List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            foreach(Car car in cars)
            {
                deleteCarComboBox.Items.Add(car.RegisterName + " " + car.Mark + " " + car.Model);
            }
            
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
            catch(WebException)
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
            List<GarageList> garageLists = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));
            foreach(GarageList garageList in garageLists)
            {
                if (garageList.ToString().Equals(garage))
                {
                    DeleteRequest("http://localhost:8080/garage/delete/" + garageList.ID, Token);
                    List<GarageList> garageListToComboBox = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));

                    deleteGarageComboBox.ItemsSource = garageListToComboBox;
                    break;
                }
            }
        }

        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        { if (registerName.Text != "" && mark.Text != "" && model.Text != "" && engine.Text != "" && power.Text != "" && garageComboBox.Text != "")
            {
                String register = registerName.Text;
                String markk = mark.Text;
                String modell = model.Text;
                String enginee = engine.Text;
                String powerr = power.Text;
                List<GarageList> garageLists = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));
                String garageNames = garageComboBox.Text;
                long garageId = -1;
                foreach (GarageList garage in garageLists)
                {
                    if (garage.ToString().Equals(garageNames))
                    {
                        garageId = garage.ID;
                        break;
                    }
                }

                String Json = "{" +
      "\"engine\": " + enginee + "," +
      "\"garageId\": " + garageId + "," +
      "\"mark\": \"" + markk + "\"," +
      "\"model\": \"" + modell + "\"," +
      "\"power\": " + powerr + "," +
      "\"registerName\": \"" + register + "\"" +
    "}";
                if (carDataGrid.SelectedItem != null)
                { 
                    List<Car> cars=null;
                    if (GetRequest("http://localhost:8080/car/", Token).Contains("mark"))
                    {
                        cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
                    }
                    foreach(Car car in cars)
                    {
                        if (car.RegisterName == register)
                        {
                            PostRequest("http://localhost:8080/car/edit/" + car.ID, Token, Json);
                            InitializeAddCarTab();
                            break;
                        }
                    }
                    
                }
                else
                {
                    System.Windows.MessageBoxResult message = System.Windows.MessageBox.Show("Are you sure that you want create new car?", "Create car confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (message == MessageBoxResult.Yes)
                    {
                        PostRequest("http://localhost:8080/car/create", Token, Json);
                        InitializeAddCarTab();                        
                    }                    
                }
                    
            }
            else
                MessageBox.Show("You have to complete all fields");
        }

        private void CarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Car> cars = null;
            if (GetRequest("http://localhost:8080/car/", Token).Contains("mark"))
            {
                cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            }
            List<GarageList> garageLists = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest("http://localhost:8080/garage/", Token));

            foreach (Car car in cars)
            {
                if(car!=null&& carDataGrid.SelectedItem!=null)
                if (car.ToString().Equals(carDataGrid.SelectedItem.ToString()))
                {
                    registerName.Text = car.RegisterName;
                    mark.Text=car.Mark;
                    model.Text=car.Model;
                    engine.Text=car.Engine.ToString();
                    power.Text=car.Power.ToString();
                    foreach(GarageList garageList in garageLists)
                    {
                        if (garageList.ID == car.GarageID)
                        {
                            garageComboBox.SelectedItem = garageList.ToString();
                        }
                    }
                }
            }
        }

        private void DeleteCarButton_Click(object sender, RoutedEventArgs e)
        {
            String carString = deleteCarComboBox.SelectedItem.ToString();

            List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            foreach (Car car in cars)
            {
                if (car.RegisterName + " " + car.Mark + " " + car.Model==carString)
                {
                    DeleteRequest("http://localhost:8080/car/delete/" + car.ID, Token);
                    InitializeDeleteCarComboBox();
                    break;
                }
            }
        }

        private void SetCarDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            setCarDetailsTab.IsEnabled = true;
            setCarDetailsTab.IsSelected = true;
            InitailizeSetCarDetailDataGrid();
        }

        public void InitailizeSetCarDetailDataGrid()
        {
            setCarDetailCarStatus.Items.Clear();
            setCarDetailCarStatus.Items.Add(Status.NEED_ATTENTION);
            setCarDetailCarStatus.Items.Add(Status.NO_FUEL);
            setCarDetailCarStatus.Items.Add(Status.ON_SERVICE);
            setCarDetailCarStatus.Items.Add(Status.READY_TO_RENT);
            setCarDetailCarStatus.Items.Add(Status.RENTED);
            setCarDetailCarStatus.Items.Add(Status.SERVICE_PLEASE);
            setCarDetailsDataGrid.ItemsSource = null;
            List<CarDetail> carDetails = JsonConvert.DeserializeObject<List<CarDetail>>(GetRequest("http://localhost:8080/cardetail/", Token));
            setCarDetailsDataGrid.ItemsSource =carDetails;
            List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            setCarDetailCar.Items.Clear();
            foreach(Car car in cars)
            {
                setCarDetailCar.Items.Add(car.Mark + " " + car.Model + " " + car.RegisterName);
            }           
        }

        private void SetCarDetails_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetCarDetailsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            long carid = -1;
            List<CarDetail> carDetails = JsonConvert.DeserializeObject<List<CarDetail>>(GetRequest("http://localhost:8080/cardetail/", Token));
            foreach(CarDetail carDetail in carDetails)
            {
                if (setCarDetailsDataGrid.SelectedItem != null)
                {
                    if (setCarDetailsDataGrid.SelectedItem.ToString() == carDetail.ToString())
                    {
                        carid = carDetail.CarID;
                    }
                }
            }


            List<Car> cars = JsonConvert.DeserializeObject<List<Car>>(GetRequest("http://localhost:8080/car/", Token));
            foreach (Car car in cars)
            {
                if(car.ID == carid)
                {
                    setCarDetailCar.SelectedItem = car.Mark + " " + car.Model + " " + car.RegisterName;
                }
            }
            
        }
    }
}
