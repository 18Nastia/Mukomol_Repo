using Microsoft.EntityFrameworkCore;
using Mukomol_Praktik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mukomol_Praktik.Pages
{
    public partial class RepeatOrderPage : Page
    {
        private MukomolContext context;
        private List<OrderView> orderViews { get; set; }
        private List<OrderView> filteredOrderViews { get; set; }

        public RepeatOrderPage()
        {
            InitializeComponent();
            context = new MukomolContext();
            orderViews = new List<OrderView>();
            filteredOrderViews = new List<OrderView>();
            FilterComboBox.Items.Add("Дате (новые сверху)");
            FilterComboBox.Items.Add("Дате (старые сверху)");
            FilterComboBox.Items.Add("Партнеру (А-Я)");
            FilterComboBox.Items.Add("Партнеру (Я-А)");
            FilterComboBox.Items.Add("Статусу (А-Я)");
            FilterComboBox.Items.Add("Статусу (Я-А)");
            FilterComboBox.Text = "";

            ViewOrders();
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
            orderViews = orderViews
        .OrderByDescending(o => o.dateOrder)
        .ToList();

            filteredOrderViews = new List<OrderView>(orderViews);
            OrdersDataGrid.ItemsSource = orderViews;
        }
        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
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
            switch (FilterComboBox.Text)
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

        private void RepeatOrder(object sender, RoutedEventArgs e)
        {
            OrderView selectedOrderView = OrdersDataGrid.SelectedItem as OrderView;

            if (selectedOrderView != null)
            {
                try
                {
                    Order originalOrder = context.Orders
                        .Include(o => o.Products)
                        .Include(o => o.IdPartnerNavigation)
                        .FirstOrDefault(o => o.IdOrder == selectedOrderView.idOrderView);

                    if (originalOrder != null)
                    {
                        List<ProductView> repeatedProducts = new List<ProductView>();

                        foreach (var product in originalOrder.Products)
                        {
                            ProductView productView = new ProductView();

                            if (product.IdFlour != null)
                            {
                                var flour = context.Flours.Find(product.IdFlour);
                                if (flour != null)
                                {
                                    productView.type = "Мука";
                                    productView.name = flour.NameFlour;
                                    productView.amount = $"{product.Amount} г";
                                    productView.originalAmount = product.Amount;
                                }
                            }
                            else if (product.IdPasta != null)
                            {
                                var pasta = context.Pasta.Find(product.IdPasta);
                                if (pasta != null)
                                {
                                    productView.type = "Макароны";
                                    productView.idProduct = pasta.IdPasta;
                                    productView.name = $"{pasta.TypePasta} {pasta.Brand}";
                                    if (pasta.Packaging != null)
                                        productView.name += $" {pasta.Packaging}г";
                                    productView.originalAmount = product.Amount;

                                    if (pasta.Packaging != null)
                                    {
                                        productView.amount = $"{product.Amount} шт";
                                    }
                                    else
                                    {
                                        productView.amount = $"{product.Amount} г";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(productView.name))
                            {
                                repeatedProducts.Add(productView);
                            }
                        }

                        string partnerName = originalOrder.IdPartnerNavigation?.NameCompany ?? "";
                        AddOrderPage addOrderPage = new AddOrderPage(repeatedProducts, partnerName);
                        NavigationService.Navigate(addOrderPage);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти выбранный заказ!",
                                        "Ошибка",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при повторении заказа: {ex.Message}",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для повторения!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
        }



    }
}