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
using System.Windows.Shapes;

namespace Mukomol_Praktik.Windows
{
    /// <summary>
    /// Логика взаимодействия для DateRangeWindow.xaml
    /// </summary>
    public partial class DateRangeWindow : Window
    {
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public DateRangeWindow()
        {
            InitializeComponent();
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            StartDate = StartDatePicker.SelectedDate;
            EndDate = EndDatePicker.SelectedDate;
            DialogResult = true;
        }
    }


}
