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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationService ns = NavigationService.GetNavigationService(this);
        }

        private void ToPartners(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
        }
        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Назад переходить некуда", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void ToOrders(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
        }

        private void ToProductPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ProductPage.xaml", UriKind.Relative));
        }

        private void ToReportPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ReportPage.xaml", UriKind.Relative));
        }

        private void ToShipmentPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ShipmentPage.xaml", UriKind.Relative));
        }
    }
}
