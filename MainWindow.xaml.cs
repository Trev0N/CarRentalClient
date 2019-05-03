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

namespace CarRentalClient
    
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const char NewChar = (char)32;
        private String token;

        public MainWindow(string token)
        {
            this.token = token;
        }

        static string GetCars(string url, string token)
        {
          
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = client.GetAsync(url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
            //Tutaj sie dzieją cudawianki xD
            // gdzie ty jestes xD

        }

        
      
        public MainWindow()
        {


            InitializeComponent();
            TextBlockFormatting();
        }
      

      public void TextBlockFormatting()
        {
            
            List<Car> car;
     
            car = JsonConvert.DeserializeObject<List<Car>>(GetCars("http://localhost:8080/car/", token));

            
            //DataGridTextColumn col1 = new DataGridTextColumn();
            //DataGridTextColumn col2 = new DataGridTextColumn();
            //DataGridTextColumn col3 = new DataGridTextColumn();
            //DataGridTextColumn col4 = new DataGridTextColumn();
            //DataGridTextColumn col5 = new DataGridTextColumn();
            //DataGridTextColumn col6 = new DataGridTextColumn();
            //DataGridTextColumn col7 = new DataGridTextColumn();
            //myDataGrid.Columns.Add(col1);
            //myDataGrid.Columns.Add(col2);
            //myDataGrid.Columns.Add(col3);
            //myDataGrid.Columns.Add(col4);
            //myDataGrid.Columns.Add(col5);
            //myDataGrid.Columns.Add(col6);
            //myDataGrid.Columns.Add(col7);
            //col1.Binding = new Binding("id");
            //col2.Binding = new Binding("registerName");
            //col3.Binding = new Binding("mark");
            //col4.Binding = new Binding("model");
            //col5.Binding = new Binding("engine");
            //col6.Binding = new Binding("power");
            //col7.Binding = new Binding("garageId");
            //col1.Header = "ID";
            //col2.Header = "RegisterName";
            //col3.Header = "mark";
            //col4.Header = "model";
            //col5.Header = "engine";
            //col6.Header = "power";
            //col7.Header = "GarageID";

            myDataGrid.ItemsSource = car;
            
            // przelacz mnie na klase car
            //otworz to okno

            // String items = myDataGrid.Items.GetItemAt(1).ToString();





        }

       
    }
}
