using Microsoft.EntityFrameworkCore;
using Mukomol_Praktik.Models;
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
    public class OrderView
    {
        public int idOrderView { get; set; }
        public string companyName { get; set; }
        public string productsOrder { get; set; }
        public string countProducts { get; set; }
        public DateOnly dateOrder { get; set; }
        public string statusOrder { get; set; }
    }
    public partial class OrderPage : Page
    {
        private MukomolContext context;
        private List<OrderView> orderViews { get; set; }
        private List<OrderView> filteredOrderViews { get; set; }

        public OrderPage()
        {
            InitializeComponent();
            context = new MukomolContext();
            orderViews = new List<OrderView>();
            filteredOrderViews = new List<OrderView>();
            ViewOrders();
            SortComboBox.Items.Add("Дате (новые сверху)");
            SortComboBox.Items.Add("Дате (старые сверху)");
            SortComboBox.Items.Add("Партнеру (А-Я)");
            SortComboBox.Items.Add("Партнеру (Я-А)");
            SortComboBox.Items.Add("Статусу (А-Я)");
            SortComboBox.Items.Add("Статусу (Я-А)");
            SortComboBox.Text = "";
        }
        public void ViewOrders()
        {
            var orders = context.Orders
        .Include(o => o.IdPartnerNavigation)
        .Include(o => o.Products)
            .ThenInclude(p => p.IdFlourNavigation)
        .Include(o => o.Products)
            .ThenInclude(p => p.IdPastaNavigation)
        .ToList();

            var allProducts = context.Products
                .Include(p => p.IdFlourNavigation)
                .Include(p => p.IdPastaNavigation)
                .ToList();

            if (context.Orders != null)
            {
                foreach (var order in orders)
                {
                    if (order.StatusOrder.ToLower() == "принят" || order.StatusOrder.ToLower() == "в обработке" || order.StatusOrder.ToLower() == "готов к отправке")
                    {
                        OrderView view = new OrderView();
                        view.idOrderView = order.IdOrder;
                        if (order.IdPartnerNavigation != null)
                        {
                            view.companyName = order.IdPartnerNavigation.NameCompany;
                        }
                        view.statusOrder = order.StatusOrder;
                        view.dateOrder = order.DateOrder;
                        var orderProducts = allProducts
                .Where(p => p.IdOrder == order.IdOrder)
                .ToList();
                        if (context.Products != null)
                        {
                            foreach (var productOrder in orderProducts)
                            {
                                if (productOrder.IdOrder == view.idOrderView)
                                {
                                    if (productOrder.IdFlour != null)
                                    {
                                        view.productsOrder += $"{productOrder.IdFlourNavigation.NameFlour}\n";
                                        view.countProducts += $"{productOrder.Amount} г\n";
                                    }
                                    else if (productOrder.IdPasta != null && productOrder.IdPastaNavigation.Packaging == null)
                                    {
                                        view.productsOrder += $"{productOrder.IdPastaNavigation.TypePasta}\n";
                                        view.countProducts += $"{productOrder.Amount} г\n";
                                    }
                                    else if (productOrder.IdPasta != null && productOrder.IdPastaNavigation.Packaging != null)
                                    {
                                        view.productsOrder += $"{productOrder.IdPastaNavigation.TypePasta} {productOrder.IdPastaNavigation.Brand} {productOrder.IdPastaNavigation.Packaging}г\n";
                                        view.countProducts += $"{productOrder.Amount} шт\n";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(view.productsOrder) && view.productsOrder.Length > 0)
                            {
                                view.productsOrder = view.productsOrder.TrimEnd('\n');
                            }

                            if (!string.IsNullOrEmpty(view.countProducts) && view.countProducts.Length > 0)
                            {
                                view.countProducts = view.countProducts.TrimEnd('\n');
                            }
                        }
                        orderViews.Add(view);
                    }

                }
            }
            OrdersDataGrid.ItemsSource = orderViews;
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
            OrderView editOrderView = OrdersDataGrid.SelectedItem as OrderView;

            if (editOrderView != null && context.Orders.Find(editOrderView.idOrderView) != null)
            {
                Order editOrder = context.Orders.Find(editOrderView.idOrderView);
                EditChoiceDoingPage editChoiceDoingPage = new EditChoiceDoingPage(editOrder);
                NavigationService.Navigate(editChoiceDoingPage);
            }
            else
            {
                MessageBox.Show("Для изменения выберите заказ",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ToPartner(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
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
            NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
        }

        private void FindOrder(object sender, RoutedEventArgs e)
        {
            string findString = FindOrderTextBox.Text;
            filteredOrderViews.Clear();
            if (!string.IsNullOrEmpty(findString))
            {
                foreach (var item in orderViews)
                {
                    bool matchesSearch = false;

                    if (!string.IsNullOrEmpty(item.companyName) &&
                        item.companyName.ToLower().Contains(findString.ToLower()))
                    {
                        matchesSearch = true;
                    }
                    else if (!string.IsNullOrEmpty(item.productsOrder) &&
                             item.productsOrder.ToLower().Contains(findString.ToLower()))
                    {
                        matchesSearch = true;
                    }
                    else if (!string.IsNullOrEmpty(item.statusOrder) &&
                             item.statusOrder.ToLower().Contains(findString.ToLower()))
                    {
                        matchesSearch = true;
                    }

                    if (matchesSearch)
                    {
                        filteredOrderViews.Add(item);
                    }
                }
                OrdersDataGrid.ItemsSource = null;
                OrdersDataGrid.ItemsSource = filteredOrderViews;
            }
            else
            {
                OrdersDataGrid.ItemsSource = null;
                OrdersDataGrid.ItemsSource = orderViews;
                filteredOrderViews = orderViews;
            }
        }

        private void FilterOrders(object sender, RoutedEventArgs e)
        {
            if (filteredOrderViews.Count == 0)
            {
                filteredOrderViews = orderViews;
            }
            switch (SortComboBox.Text)
            {
                case "Дате (новые сверху)":
                    filteredOrderViews = filteredOrderViews
                        .OrderByDescending(o => o.dateOrder)
                        .ToList();
                    break;
                case "Дате (старые сверху)":
                    filteredOrderViews = filteredOrderViews
                        .OrderBy(o => o.dateOrder)
                        .ToList();
                    break;
                case "Партнеру (А-Я)":
                    filteredOrderViews = filteredOrderViews
                        .OrderBy(o => o.companyName)
                        .ToList();
                    break;
                case "Партнеру (Я-А)":
                    filteredOrderViews = filteredOrderViews
                        .OrderByDescending(o => o.companyName)
                        .ToList();
                    break;
                case "Статусу (А-Я)":
                    filteredOrderViews = filteredOrderViews
                        .OrderBy(o => o.statusOrder)
                        .ToList();
                    break;
                case "Статусу (Я-А)":
                    filteredOrderViews = filteredOrderViews
                        .OrderByDescending(o => o.statusOrder)
                        .ToList();
                    break;
            }
            OrdersDataGrid.ItemsSource = null;
            OrdersDataGrid.ItemsSource = filteredOrderViews;
        }
    }
}
