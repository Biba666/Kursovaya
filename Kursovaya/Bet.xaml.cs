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
        public Bet(string teamName, string matchDate, string ratio)
        {
            InitializeComponent();

            _team.Content = teamName;
            _date.Content = matchDate;
            _ratio.Content = ratio;
        }

        private void moneyBoxKeyDown(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            sum.Content = "Итого: ";

            if (float.TryParse(tb.Text, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out float money)) {
                sum.Content += (money * float.Parse(_ratio.Content.ToString(), CultureInfo.InvariantCulture.NumberFormat)).ToString("F2");
            } else {
                sum.Content += "0";
            }
        }

        private void NumValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch(((TextBox)sender).Text + e.Text);
        }

        private readonly Regex regex = new Regex(@"^[0-9]+(,|.)?[0-9]?[0-9]?$");
    }
}
