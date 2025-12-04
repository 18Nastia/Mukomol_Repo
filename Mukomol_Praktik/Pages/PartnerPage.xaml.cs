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
    /// Логика взаимодействия для PartnerPage.xaml
    /// </summary>
    public partial class PartnerPage : Page
    {
        public PartnerPage()
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

        private void ToAddPartner(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AddPartnerPage.xaml", UriKind.Relative));
        }

        private void ToDeletePartner(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DeletePartnerPage.xaml", UriKind.Relative));
        }

        private void ToOrder(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
        }
    }
}
