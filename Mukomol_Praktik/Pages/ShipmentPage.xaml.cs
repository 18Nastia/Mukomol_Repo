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

namespace Mukomol_Praktik.Pages
{
    /// <summary>
    /// Логика взаимодействия для ShipmentPage.xaml
    /// </summary>
    public partial class ShipmentPage : Page
    {
        public ShipmentPage()
        {
            InitializeComponent();
        }

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                // Можно, например, вывести сообщение или просто ничего не делать
                MessageBox.Show("Нет предыдущей страницы для возврата.", "Навигация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ToProduct(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ProductPage());
        }
        private void ToReport(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ReportPage());
        }
    }
}
