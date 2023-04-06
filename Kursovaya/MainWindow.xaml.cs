using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net.Http;
using Newtonsoft.Json;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Row
    {
        public string Game { get; set; }
        public string Date { get; set; }
        public string FirstWin { get; set; }
        public string Spare { get; set; }
        public string SecondWin { get; set; }
    }

    public partial class MainWindow : Window
    {
        static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();

            string apiKey = "7235fb8523a840e9979bd25faff57198";
            string url = "https://api.football-data.org/v4/matches";

            client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode) {
                string responseBody = response.Content.ReadAsStringAsync().Result;

                dynamic data = JsonConvert.DeserializeObject(responseBody);
            } else {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
