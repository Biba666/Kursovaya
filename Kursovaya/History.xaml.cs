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
    public class Match
    {
        public string GameName { get; set; }
        public DateTime Date { get; set; }
        public string Team { get; set; }
        public float Money { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для History.xaml
    /// </summary>
    public partial class History : Window
    {
        public History()
        {
            InitializeComponent();

            _filter.SelectedIndex = 0;

            DatagridUpdate();

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DatagridUpdate();
        }

        private void DatagridUpdate()
        {
            _info.Items.Clear();

            List<Match> data = new DatabaseConnection().GetMatches("buba", _filter.SelectedValue.ToString());

            foreach (Match row in data)
            {
                _ = _info.Items.Add(row);
            }
        }
    }
}
