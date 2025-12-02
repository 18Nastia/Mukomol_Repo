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
    /// Логика взаимодействия для EditChoiceOrderPage.xaml
    /// </summary>
    public partial class EditChoiceOrderPage : Page
    {
        public EditChoiceOrderPage()
        {
            InitializeComponent();
        }
        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ToChoiceDoing(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/EditChoiceDoingPage.xaml", UriKind.Relative));
        }
    }
}
