using System;
using System.Windows;

namespace Mukomol_Praktik.Pages
{
    public partial class ShippingDateWindow : Window
    {
        public DateTime? SelectedDate { get; private set; }

        public ShippingDateWindow()
        {
            InitializeComponent();
            ShippingDatePicker.SelectedDate = DateTime.Today;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = ShippingDatePicker.SelectedDate;
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
