using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kursovaya
{
    /// <summary>
    /// Логика взаимодействия для DepositMoney.xaml
    /// </summary>
    public partial class DepositMoney : Window
    {
        public DepositMoney(float money)
        {
            InitializeComponent();

            _balance.Content += money.ToString();
        }

        private void NumValidator(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !regex.IsMatch(((TextBox)sender).Text + e.Text);
        }

        private readonly Regex regex = new(@"^[0-9]+(,|.)?[0-9]?[0-9]?$");

        private void MakeDeposit(object sender, RoutedEventArgs e)
        {
            _ = new DatabaseConnection().AddMoney(float.Parse(_value.Text));

            DialogResult = true;

            Close();
        }
    }
}
