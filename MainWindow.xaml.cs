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

namespace CarRentalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        String token;

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

                String abrakadabra =  response.Content.ReadAsStringAsync().Result;

                return response.Content.ReadAsStringAsync().Result;
            }


        }


      
        public MainWindow()
        {
            
            InitializeComponent();
            TextBlockFormatting();
        }


      public void TextBlockFormatting()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.Margin = new Thickness(15);
           // textBlock.Text = GetCars("http://localhost:8080/car/", token);
            //  textBlock.Inlines.Add(GetCars("http://localhost:8080/car/", token));
            this.Content = textBlock;
        }

        private void TextBlock_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            GetCars("http://localhost:8080/car/", token);
        }
    }
}
