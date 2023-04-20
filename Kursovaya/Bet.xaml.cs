using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Globalization;


namespace Kursovaya
{
    /// <summary>
    /// Логика взаимодействия для Bet.xaml
    /// </summary>
    public partial class Bet : Window
    {
        private readonly int _gameId;
        private readonly string _side;
        public float _balance;

        public Bet(int gameId, string side, string teamName, string matchDate, string ratio, float balance)
        {
            InitializeComponent();

            _gameId = gameId;
            _side = side;
            _team.Content = teamName;
            _date.Content = matchDate;
            _ratio.Content = ratio;
            _balance = balance;
        }

        private void moneyBoxKeyDown(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            sum.Content = "Итого: ";

            if (float.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out float money)) {
                sum.Content += (money * float.Parse(_ratio.Content.ToString(), CultureInfo.InvariantCulture.NumberFormat)).ToString("F2", CultureInfo.InvariantCulture);
            } else {
                sum.Content += "0";
            }
        }

        private void NumValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch(((TextBox)sender).Text + e.Text);
        }

        private readonly Regex regex = new(@"^[0-9]+(,|.)?[0-9]?[0-9]?$");

        private void MakeBet(object sender, RoutedEventArgs e)
        {
            string sumText = sum.Content.ToString().Substring(7);

            _balance -= float.Parse(_money.Text, CultureInfo.InvariantCulture.NumberFormat);

            if (sumText == "0") 
            {
                return;
            }
            else if (_balance < 0)
            {
                _ = MessageBox.Show("Недостаточно денег!");
                return;
            }

            DatabaseConnection dc = new DatabaseConnection();
            dc.RunCommand(
                "INSERT INTO public.bet(user_id,game_id,side,amount)" +
                "VALUES (1," + _gameId + ",'" + _side + "','" + sumText + "'::float);"
            );

            DialogResult = true;

            Close();
        }
    }
}
