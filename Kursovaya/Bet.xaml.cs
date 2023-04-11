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
    }
}
