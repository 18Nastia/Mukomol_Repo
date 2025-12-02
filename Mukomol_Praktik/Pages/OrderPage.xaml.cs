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
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
        }
        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void ToMain(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }
        private void ToAddOrder(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AddOrderPage.xaml", UriKind.Relative));
        }

        private void ToEditOrder(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/EditChoiceOrderPage.xaml", UriKind.Relative));
        }

        private void ToPartner(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
        }
    }
}
