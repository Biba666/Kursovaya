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
using System.Diagnostics;
using System.Globalization;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Row
    {
        public int GameId { get; set; }
        public string Game { get; set; }
        public string Date { get; set; }
        public string FirstWin { get; set; }
        public string Spare { get; set; }
        public string SecondWin { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
    }

    public partial class MainWindow : Window
    {
        static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();

            _userName.Content = new DatabaseConnection().GetUsername("buba");
            _balance.Content += new DatabaseConnection().GetBalance("buba").ToString();

            string apiKey = "7235fb8523a840e9979bd25faff57198";
            //string url = "https://api.football-data.org/v4/matches";
            string url = "https://api.football-data.org/v4/competitions/CL/matches?status=SCHEDULED";

            client.DefaultRequestHeaders.Add("X-Auth-Token", apiKey);

            HttpResponseMessage response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode) {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            addRowsToDatagrid(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));

            url = "https://api.football-data.org/v4/competitions/BL1/matches?status=SCHEDULED";

            response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            addRowsToDatagrid(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));

            url = "https://api.football-data.org/v4/competitions/SA/matches?status=SCHEDULED";

            response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            addRowsToDatagrid(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));

            url = "https://api.football-data.org/v4/competitions/PD/matches?status=SCHEDULED";

            response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            addRowsToDatagrid(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));

            url = "https://api.football-data.org/v4/competitions/DED/matches?status=SCHEDULED";

            response = client.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }

            addRowsToDatagrid(JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result));
        }

        private void addRowsToDatagrid(dynamic values)
        {
            string matchName, date, homeWin, draw, awayWin;
            int gameId;

            foreach (dynamic match in values.matches) {

                matchName = (string)match.homeTeam.name + " - " + (string)match.awayTeam.name;
                date = (string)match.utcDate;
                homeWin = (string)match.odds.homeWin;
                draw = (string)match.odds.draw;
                awayWin = (string)match.odds.awayWin;
                gameId = match.id.ToObject<int>();

                if (matchName == null || date == null || homeWin == null || draw == null || awayWin == null) {
                    continue;
                }

                biba.Items.Add(new Row { GameId = gameId, Game = matchName, Date = date, FirstWin = homeWin, Spare = draw, SecondWin = awayWin, HomeTeam = (string)match.homeTeam.name, AwayTeam = (string)match.awayTeam.name });
            }
        }

        protected static object getValueFromObjectByName(object source, string propertyName)
        {
            return source.GetType().GetProperty(propertyName).GetValue(source, null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void betButton(object sender, RoutedEventArgs e)
        {
            Row r = ((FrameworkElement)sender).DataContext as Row;

            Button button = sender as Button;

            Bet b = new Bet(
                r.GameId,
                button.Name,
                (button.Name == "HomeTeam") ? r.HomeTeam :
                (button.Name == "AwayTeam") ? r.AwayTeam :
                "Ничья",
                r.Date,
                button.Content.ToString(),
                float.Parse(_balance.Content.ToString()[16..])
            ) {
                Owner = this
            };

            bool? biba = b.ShowDialog();

            if (biba == true)
            {
                _ = new DatabaseConnection().RunCommand("UPDATE public.user u SET balance = " + b._balance.ToString("F2", CultureInfo.InvariantCulture) + " WHERE u.login = 'buba';");

                _balance.Content = "Баланс: " + b._balance.ToString();
            }

        }
    }
}
