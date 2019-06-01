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
    /// Logika dla okna panelu administratora
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char NewChar = (char)32;
        private String Token;
        private const string address = "https://carrental-wsiz.herokuapp.com/";

        /// <summary>
        /// Konstruktor klasy z tokenem aby moc przekazac z okna do okna token potrzebny do pracy aplikacji
        /// </summary>
        /// <param name="Token">zawiera token do uzyskania zasobow serwera</param>
        public MainWindow(string Token)
        {
            this.Token = Token;
        }
        /// <summary>
        /// Pusty konstruktor iniciujacy okno aplikacji
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            TextBlockFormatting();
            CenterWindowOnScreen();
        }

        /// <summary>
        /// Uzupełnia datagrid danymi z serwera
        /// </summary>
        public void TextBlockFormatting()
        {
            List<Car> car = RequestToListCar();
            myDataGrid.ItemsSource = car;
        }
        /// <summary>
        /// Zapytanie GET do serwera, aby uzyskać potrzebne informacje
        /// </summary>
        /// <returns>liste samochodów</returns>
        public List<Car> RequestToListCar()
        {
            if (GetRequest(address + "car/", Token).Contains("mark"))
                return JsonConvert.DeserializeObject<List<Car>>(GetRequest(address + "car/", Token));
            else
                return null;
        }

        /// <summary>
        /// Centruje okienko na środku ekranu naszego komputera
        /// </summary>
        public void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        /// <summary>
        /// Zapytanie GET do serwera, aby uzyskać potrzebne informacje
        /// </summary>
        /// <returns>liste garaży</returns>
        public List<GarageList> RequestToGarageList()
        {
            if (GetRequest(address + "garage/", Token).Contains("name"))
                return JsonConvert.DeserializeObject<List<GarageList>>(GetRequest(address + "garage/", Token));
            else
                return null;
        }
    

       
       //ADD/EDIT GARAGE TAB
       /// <summary>
       /// Wypełnia danymi datagrid dla garaży
       /// </summary>
        public void InitializeAddGarageTab()
        {
            List<GarageList> garageList = RequestToGarageList();
            addGarageDataGrid.ItemsSource=garageList;
        }
        /// <summary>
        /// Kliknięcie tego guzika pobiera wszystkie dane z całego formularza i wysyła je PUT-em (edycja) lub POST-em (tworzenie) do serwera
        /// </summary>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            String Name = addGarageName.Text;
            String Address = addGarageAddress.Text;


            List<GarageList> garageList = RequestToGarageList();

            if (Name != null && Address != null && Name != "" && Address !="")
            {
                                             
                String json = "{" +
                                 "\"address\": \"" + Address + "\"," +
                                 "\"name\": \""+Name+"\""
                                + "}";
                if(garageList!=null)
                foreach (GarageList garage in garageList)
                {

                    if (addGarageDataGrid.SelectedItem != null && addGarageDataGrid.SelectedItem.ToString().Equals(garage.ToString()))
                    {
                            try
                            {
                                PutRequest(address+"garage/edit/" + garage.ID, Token, json);
                            }
                            catch (WebException)
                            {
                                MessageBox.Show("Garage name isn't available, please change it!");
                            }
                            break;
                    }
                    else
                    {
                        System.Windows.MessageBoxResult message = System.Windows.MessageBox.Show("Are you sure that you want create new garage?","Create garage confirmation", System.Windows.MessageBoxButton.YesNo);
                        if (message == MessageBoxResult.Yes)
                        {
                            PostRequest(address+"garage/create", Token, json);
                            break;
                        }
                        else
                            break;
                    }
                }
                else
                    PostRequest(address+"garage/create", Token, json);
                
                InitializeAddGarageTab();
            }
            else
                MessageBox.Show("You have to complete all fields. ");
        }

        //END


            //Add/edit car
            /// <summary>
            /// Wypełnia datagrid dla samochodów danymi
            /// </summary>
        public void InitializeAddCarTab()
        {
             List<Car> cars = RequestToListCar();
            carDataGrid.ItemsSource = cars;

            List<GarageList> garageLists = RequestToGarageList();
            garageComboBox.ItemsSource = garageLists;

        }

        /// <summary>
        /// Otwiera zakladke dodawania/edycji samochodu
        /// </summary>
        public void Button_Create_Car(object sender, RoutedEventArgs e)
        {
            addCarTab.IsEnabled = true;
            addCarTab.IsSelected = true;
            InitializeAddCarTab();
        }

        /// <summary>
        /// Otwiera zakladke usuwania samochodu
        /// </summary>
        public void Button_Delete_Car(object sender, RoutedEventArgs e)
        {
            deleteCarTab.IsEnabled = true;
            deleteCarTab.IsSelected = true;
            InitializeDeleteCarComboBox();
        }
        /// <summary>
        /// Inicjuje combobox do usuniecia samochodu
        /// </summary>
        public void InitializeDeleteCarComboBox()
        {
            deleteCarComboBox.Items.Clear();
            List<Car> cars = RequestToListCar();
            if (cars != null)
            {
                foreach (Car car in cars)
                {
                    deleteCarComboBox.Items.Add(car.RegisterName + " " + car.Mark + " " + car.Model);
                }
            }
            
        }
        /// <summary>
        /// Otwiera zakladke usuwania garażu oraz inicjuje combobox
        /// </summary>
        public void Button_Delete_Garage(object sender, RoutedEventArgs e)
        {
            deleteGarageTab.IsEnabled = true;
            deleteGarageTab.IsSelected = true;
            List<GarageList> garageLists = RequestToGarageList();

            deleteGarageComboBox.ItemsSource = garageLists;
        }


        /// <summary>
        /// Otwiera zakladke tworzenia/edycji garazu, wraz z zainicjalizowaniem wszystkich potrzebnych komponentow
        /// </summary>
        public void Button_Add_Garage(object sender, RoutedEventArgs e)
        {
            addGarageTab.IsEnabled = true;
            addGarageTab.IsSelected = true;
            InitializeAddGarageTab();
        }

        /// <summary>
        /// Potrzebna logika do obsłużenia wylogowywania z naszego serwisu, wysyłamy token do naszego serwera metodą DELETE
        /// </summary>
        public void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequest(address+"sign-out", Token);
            LoginScreen login = new LoginScreen();
            login.InitializeComponent();
            login.Show();
            this.Close();
        }

        //REQUESTS

            /// <summary>
            /// Templatka do requesta GET do naszego serwera
            /// </summary>
            /// <returns>zwraca jsona w stringu</returns>
        public static string GetRequest(string url, string Token)
        {

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                var response = client.GetAsync(url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }


        }
        /// <summary>
        /// Templatka do requesta POST do naszego serwera
        /// </summary>
        public void PostRequest(String url, String token, String json)
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

        /// <summary>
        /// Templatka do requesta PUT do naszego serwera
        /// </summary>
        public void PutRequest(String url, String token, String json)
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
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
            }
        /// <summary>
        /// Templatka do requesta DELETE do naszego serwera
        /// </summary>
        /// <param name="url">adres do serwera</param>
        /// <param name="Token">token z logowania</param>
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
        /// Wybranie rekordu w datagridzie skutkuje uzupełnieniem formularza tymi danymi
        /// </summary>
        private void AddGarageDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text=null;
            if (addGarageDataGrid.SelectedItem != null)
            {
                text = addGarageDataGrid.SelectedItem.ToString();
            }
            List<GarageList> garageList;
            garageList = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest(address+"garage/", Token));
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
        /// <summary>
        /// Obsługa usuwania garażu, tzn request DELETE do serwera z ID garażu
        /// </summary>
        private void Delete_Garage_Click(object sender, RoutedEventArgs e)
        {
            String garage = deleteGarageComboBox.SelectedItem.ToString();
            List<GarageList> garageLists = RequestToGarageList();
            foreach(GarageList garageList in garageLists)
            {
                if (garageList.ToString().Equals(garage))
                {
                    DeleteRequest(address+"garage/delete/" + garageList.ID, Token);
                    List<GarageList> garageListToComboBox = RequestToGarageList();

                    deleteGarageComboBox.ItemsSource = garageListToComboBox;
                    break;
                }
            }
        }
        /// <summary>
        /// Metoda która przygotowuje JSON'a i wysyła request'a POST do serwera
        /// </summary>
        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        { if (registerName.Text != "" && mark.Text != "" && model.Text != "" && engine.Text != "" && power.Text != "" && garageComboBox.Text != "")
            {
                String register = registerName.Text;
                String markk = mark.Text;
                String modell = model.Text;
                String enginee = engine.Text;
                String powerr = power.Text;
                List<GarageList> garageLists = JsonConvert.DeserializeObject<List<GarageList>>(GetRequest(address+"garage/", Token));
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
                    List<Car> cars= RequestToListCar();
                    foreach(Car car in cars)
                    {
                        if (carDataGrid.SelectedItem.ToString()==car.ToString())
                        {
                            PostRequest(address+"car/edit/" + car.ID, Token, Json);
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
                        PostRequest(address+"car/create", Token, Json);
                        InitializeAddCarTab();                        
                    }                    
                }
                    
            }
            else
                MessageBox.Show("You have to complete all fields");
        }
        /// <summary>
        ///  Wybranie rekordu w datagridzie skutkuje uzupełnieniem formularza tymi danymi
        /// </summary>
        private void CarDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            List<Car> cars = RequestToListCar();
            List<GarageList> garageLists = RequestToGarageList();

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
        /// <summary>
        /// Metoda który wyszukuje jaki chcemy pojazd usunąć i wysyła requesta DELETE do serwera
        /// </summary>
        private void DeleteCarButton_Click(object sender, RoutedEventArgs e)
        {
            String carString = deleteCarComboBox.SelectedItem.ToString();

            List<Car> cars = RequestToListCar();
            foreach (Car car in cars)
            {
                if (car.RegisterName + " " + car.Mark + " " + car.Model==carString)
                {
                    DeleteRequest(address+"car/delete/" + car.ID, Token);
                    InitializeDeleteCarComboBox();
                    break;
                }
            }
        }
        /// <summary>
        /// Metoda do obsługi przycisku, który przełącza widok na setcardetail 
        /// </summary>
        private void SetCarDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            setCarDetailsTab.IsEnabled = true;
            setCarDetailsTab.IsSelected = true;
            InitailizeSetCarDetailDataGrid();
        }
        /// <summary>
        /// Metoda inicializująca okienko SetCarDetail 
        /// </summary>
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
            List<CarDetail> carDetails = JsonConvert.DeserializeObject<List<CarDetail>>(GetRequest(address+"cardetail/", Token));
            setCarDetailsDataGrid.ItemsSource =carDetails;
            List<Car> cars = RequestToListCar();
            setCarDetailCar.Items.Clear();
            foreach(Car car in cars)
            {
                setCarDetailCar.Items.Add(car.Mark + " " + car.Model + " " + car.RegisterName);
            }           
        }
        /// <summary>
        /// Metoda wysytworzaca JSON'a i wysyłająca go do serwera
        /// </summary>
        private void SetCarDetails_Click(object sender, RoutedEventArgs e)
        {
            long carid=-1;
            Boolean exists=false;

            if (setCarDetailsPrice.Text != null && setCarDetailsMileage.Text != null && setCarDetailCarStatus.SelectedItem != null && setCarDetailCar.SelectedItem != null)
            {
                List<Car> cars = RequestToListCar();
                foreach (Car car in cars)
                {
                    if (car.Mark + " " + car.Model + " " + car.RegisterName == setCarDetailCar.SelectedItem.ToString())
                    {
                        carid = car.ID;
                        break;
                    }
                }
                List<CarDetail> carDetails = JsonConvert.DeserializeObject<List<CarDetail>>(GetRequest(address+"cardetail/", Token));
                foreach (CarDetail carDetail in carDetails)
                {
                    if (carDetail.CarID == carid)
                    {
                        exists = true;
                        break;
                    }
                }
                String Json = "{" +
          "\"carId\": " + carid + "," +
          "\"mileage\": " + setCarDetailsMileage.Text + "," +
          "\"price\": " + setCarDetailsPrice.Text + "," +
          "\"statusEnum\":\"" + setCarDetailCarStatus.SelectedItem.ToString() + "\"" +
          "}";
                if (exists)
                {
                    PutRequest(address+"cardetail/update", Token, Json);
                }
                else
                {
                    PostRequest(address+"cardetail/create", Token, Json);
                }
                InitailizeSetCarDetailDataGrid();
            }
            else
                MessageBox.Show("You have to complete all fields");
        }
        /// <summary>
        ///  Wybranie rekordu w datagridzie skutkuje uzupełnieniem formularza tymi danymi
        /// </summary>
        private void SetCarDetailsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            long carid = -1;
            List<CarDetail> carDetails = JsonConvert.DeserializeObject<List<CarDetail>>(GetRequest(address+"cardetail/", Token));
            foreach(CarDetail carDetail in carDetails)
            {
                if (setCarDetailsDataGrid.SelectedItem != null)
                {
                    if (setCarDetailsDataGrid.SelectedItem.ToString() == carDetail.ToString())
                    {
                        carid = carDetail.CarID;
                        setCarDetailCarStatus.SelectedItem = carDetail.status;
                        setCarDetailsMileage.Text = carDetail.mileage.ToString();
                        setCarDetailsPrice.Text = carDetail.price.ToString();
                        break;

                    }

                }
            }
            List<Car> cars = RequestToListCar();
            foreach (Car car in cars)
            {
                if(car.ID == carid)
                {
                    setCarDetailCar.SelectedItem = car.Mark + " " + car.Model + " " + car.RegisterName;
                    break;
                }
            }
        }
    }
}
