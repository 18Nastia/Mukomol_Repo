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
    /// Логика взаимодействия для EditChoiceDoingPage.xaml
    /// </summary>
    public partial class EditChoiceDoingPage : Page
    {
        public EditChoiceDoingPage()
        {
            InitializeComponent();
        }

        private void ToAddProduct(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AddProductOrderPage.xaml", UriKind.Relative));
        }
        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ToEditStatus(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/EditStatusOrderPage.xaml", UriKind.Relative));
        }

        private void ToEditCountProduct(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/EditCountProductPage.xaml", UriKind.Relative));
        }
    }
}
