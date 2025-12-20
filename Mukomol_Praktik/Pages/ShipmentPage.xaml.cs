using Microsoft.EntityFrameworkCore;
using Mukomol_Praktik.Models;
using Mukomol_Praktik.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Mukomol_Praktik.Pages
{
    public class ShipmentView
    {
        public int IdOrder { get; set; }
        public string Client { get; set; }
        public string Products { get; set; }
        public string Quantity { get; set; }

        public DateTime OrderDateRaw { get; set; }
        public string OrderDate { get; set; }

        public string ShippingDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }

    public partial class ShipmentPage : Page
    {
        private MukomolContext context;
        private List<ShipmentView> shipments;

        public ShipmentPage()
        {
            InitializeComponent();
            context = new MukomolContext();
            shipments = new List<ShipmentView>();
            LoadShipments();
        }

        private void LoadShipments()
        {
            shipments.Clear();

            var orders = context.Orders
                .Include(o => o.IdPartnerNavigation)
                .Include(o => o.Products)
                    .ThenInclude(p => p.IdFlourNavigation)
                .Include(o => o.Products)
                    .ThenInclude(p => p.IdPastaNavigation)
                .Where(o =>
                    o.StatusOrder == "Отгружен" ||
                    o.StatusOrder == "Не отгружен" ||
                    o.StatusOrder == "Отправлен" ||
                    o.StatusOrder == "Отменен")
                .ToList();

            var allProducts = context.Products
                .Include(p => p.IdFlourNavigation)
                .Include(p => p.IdPastaNavigation)
                .ToList();

            foreach (var order in orders)
            {
                ShipmentView view = new ShipmentView
                {
                    IdOrder = order.IdOrder,
                    Client = order.IdPartnerNavigation?.NameCompany ?? "",
                    Status = order.StatusOrder,

                    OrderDateRaw = order.DateOrder.ToDateTime(TimeOnly.MinValue),
                    OrderDate = order.DateOrder.ToString("dd.MM.yyyy"),

                    ShippingDate = order.DateShipping.HasValue
                        ? order.DateShipping.Value.ToString("dd.MM.yyyy")
                        : "",

                    Comment = order.CommentOrder ?? "",
                    Products = "",
                    Quantity = ""
                };

                var orderProducts = allProducts
                    .Where(p => p.IdOrder == order.IdOrder)
                    .ToList();

                foreach (var product in orderProducts)
                {
                    if (product.IdFlour != null)
                    {
                        view.Products += $"{product.IdFlourNavigation.NameFlour}\n";
                        view.Quantity += $"{product.Amount} г\n";
                    }
                    else if (product.IdPasta != null)
                    {
                        if (!product.IdPastaNavigation.Packaging.HasValue)
                        {
                            view.Products += $"{product.IdPastaNavigation.TypePasta}\n";
                            view.Quantity += $"{product.Amount} г\n";
                        }
                        else
                        {
                            view.Products +=
                                $"{product.IdPastaNavigation.TypePasta} " +
                                $"{product.IdPastaNavigation.Brand} " +
                                $"{product.IdPastaNavigation.Packaging.Value}г\n";

                            view.Quantity += $"{product.Amount} шт\n";
                        }
                    }
                }

                view.Products = view.Products.TrimEnd('\n');
                view.Quantity = view.Quantity.TrimEnd('\n');

                shipments.Add(view);
            }

            ShipmentsDataGrid.ItemsSource = shipments;
        }

        // ================= НАВИГАЦИЯ =================

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
                NavigationService.GoBack();
            else
                MessageBox.Show("Назад переходить некуда");
        }

        private void ToMain(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void ToPartner(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
        }

        private void ToProduct(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ProductPage.xaml", UriKind.Relative));
        }

        private void ToOrder(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
        }

        private void ToReport(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ReportPage.xaml", UriKind.Relative));
        }

        // ================= ПОИСК =================

        private void ToSearchShipment(object sender, RoutedEventArgs e)
        {
            string query = FindShipmentTextBox.Text.ToLower();

            ShipmentsDataGrid.ItemsSource = shipments
                .Where(s =>
                    s.Client.ToLower().Contains(query) ||
                    s.Products.ToLower().Contains(query))
                .ToList();
        }

        private void ToFilterShipment(object sender, RoutedEventArgs e)
        {
            ShipmentsDataGrid.ItemsSource = shipments;

            foreach (var rb in FindVisualChildren<RadioButton>(this))
                rb.IsChecked = false;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
    where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    yield return typedChild;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }


        // ================= СТАТУС =================

        private void ToMarkShipped(object sender, RoutedEventArgs e)
        {
            if (ShipmentsDataGrid.SelectedItem is ShipmentView selected)
            {
                var window = new ShippingDateWindow { Owner = Window.GetWindow(this) };

                if (window.ShowDialog() == true && window.SelectedDate.HasValue)
                {
                    var order = context.Orders.First(o => o.IdOrder == selected.IdOrder);
                    order.StatusOrder = "Отгружен";
                    order.DateShipping = DateOnly.FromDateTime(window.SelectedDate.Value);

                    context.SaveChanges();

                    selected.Status = "Отгружен";
                    selected.ShippingDate = window.SelectedDate.Value.ToString("dd.MM.yyyy");
                    ShipmentsDataGrid.Items.Refresh();
                }
            }
        }

        private void ToMarkNotShipped(object sender, RoutedEventArgs e)
        {
            if (ShipmentsDataGrid.SelectedItem is ShipmentView selected)
            {
                var window = new CommentWindow { Owner = Window.GetWindow(this) };

                if (window.ShowDialog() == true)
                {
                    var order = context.Orders.First(o => o.IdOrder == selected.IdOrder);
                    order.StatusOrder = "Не отгружен";
                    order.CommentOrder = window.Comment;

                    context.SaveChanges();

                    selected.Status = "Не отгружен";
                    selected.Comment = window.Comment;
                    ShipmentsDataGrid.Items.Refresh();
                }
            }
        }

        // ================= ФИЛЬТР ПО ДАТЕ =================

        private void FilterByDate(Func<DateTime, bool> predicate)
        {
            ShipmentsDataGrid.ItemsSource = shipments
                .Where(s => predicate(s.OrderDateRaw))
                .ToList();
        }

        private void FilterDay(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;
            FilterByDate(d => d.Date == today);
        }

        private void FilterWeek(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;

            // Определяем понедельник текущей недели
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStart = today.AddDays(-diff).Date;
            DateTime weekEnd = weekStart.AddDays(6).Date;

            FilterByDate(d => d.Date >= weekStart && d.Date <= weekEnd);
        }


        private void FilterMonth(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;

            DateTime monthStart = new DateTime(today.Year, today.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

            FilterByDate(d => d.Date >= monthStart && d.Date <= monthEnd);
        }


        private void FilterYear(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Today;

            DateTime yearStart = new DateTime(today.Year, 1, 1);
            DateTime yearEnd = new DateTime(today.Year, 12, 31);

            FilterByDate(d => d.Date >= yearStart && d.Date <= yearEnd);
        }


        private void FilterRange(object sender, RoutedEventArgs e)
        {
            var window = new DateRangeWindow { Owner = Window.GetWindow(this) };

            if (window.ShowDialog() == true &&
                window.StartDate.HasValue &&
                window.EndDate.HasValue)
            {
                FilterByDate(d =>
                    d >= window.StartDate.Value.Date &&
                    d <= window.EndDate.Value.Date);
            }
        }
    }
}
