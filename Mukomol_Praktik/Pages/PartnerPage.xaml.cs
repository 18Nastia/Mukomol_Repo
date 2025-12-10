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
using Mukomol_Praktik.Models;

namespace Mukomol_Praktik.Pages
{
    /// <summary>
    /// Логика взаимодействия для PartnerPage.xaml
    /// </summary>
    public partial class PartnerPage : Page
    {
        public List<Partner> Partners {  get; set; }
        private MukomolContext context;
        public PartnerPage()
        {
            InitializeComponent();
            context = new MukomolContext();
            Partners = new List<Partner>();
            ViewPartners();
        }
        public void ViewPartners()
        {
            try
            {
                var partners = context.Partners.ToList();
                PartnersDataGrid.ItemsSource = partners;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
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


        private void ToMain(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }
        private void ToOrder(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
        }
        private void ToProduct(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ProductPage.xaml", UriKind.Relative));
        }
        private void ToShipment(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ShipmentPage.xaml", UriKind.Relative));
        }
        private void ToReport(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ReportPage.xaml", UriKind.Relative));
        }

        private void ToAddPartner(object sender, RoutedEventArgs e)
        {
            AddPartnerPage addPartnerPage = new AddPartnerPage(null);
            NavigationService.Navigate(addPartnerPage);
        }
        private void ToEditPartner(object sender, RoutedEventArgs e)
        {
            Partner editPartner = PartnersDataGrid.SelectedItem as Partner;
            if (editPartner != null)
            {
                AddPartnerPage addPartnerPage = new AddPartnerPage(editPartner);
                NavigationService.Navigate(addPartnerPage);
            }
            else
            {
                MessageBox.Show("Для изменения выберите партнера",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            
        }

        private void ToDeletePartner(object sender, RoutedEventArgs e)
        {
            Partner delPartner = PartnersDataGrid.SelectedItem as Partner;
            if (delPartner != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранного партнера?",
                                                      "Подтверждение",
                                                      MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var order in context.Orders.ToList())
                    {
                        if (order.IdPartnerNavigation == delPartner)
                        {
                            context.Orders.Remove(order);
                        }
                    }
                    context.Partners.Remove(delPartner);
                    context.SaveChanges();
                    NavigationService.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Для удаления выберите партнера",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            }          
            
        }
      
    }
}
