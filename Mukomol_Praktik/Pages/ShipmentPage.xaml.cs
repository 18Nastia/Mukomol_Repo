using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mukomol_Praktik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mukomol_Praktik.Pages
{
    public class ShipmentView
    {
        public int IdOrder { get; set; }
        public string Client { get; set; }
        public string Products { get; set; }
        public string Quantity { get; set; }
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
                    OrderDate = order.DateOrder.ToString("dd.MM.yyyy"),
                    ShippingDate = order.DateShipping.HasValue ? order.DateShipping.Value.ToString("dd.MM.yyyy") : "",
                    Comment = order.CommentOrder ?? "",
                    Products = "",
                    Quantity = ""
                };

                var orderProducts = allProducts.Where(p => p.IdOrder == order.IdOrder).ToList();

                foreach (var product in orderProducts)
                {
                    if (product.IdFlour != null)
                    {
                        view.Products += $"{product.IdFlourNavigation.NameFlour}\n";
                        view.Quantity += $"{product.Amount} г\n";
                    }
                    else if (product.IdPasta != null)
                    {
                        // Packaging у пасты - int? (по твоей модели)
                        if (!product.IdPastaNavigation.Packaging.HasValue)
                        {
                            view.Products += $"{product.IdPastaNavigation.TypePasta}\n";
                            view.Quantity += $"{product.Amount} г\n";
                        }
                        else
                        {
                            view.Products += $"{product.IdPastaNavigation.TypePasta} {product.IdPastaNavigation.Brand} {product.IdPastaNavigation.Packaging.Value}г\n";
                            view.Quantity += $"{product.Amount} шт\n";
                        }
                    }
                }

                // Убираем последний символ переноса строки
                if (view.Products.Length > 0) view.Products = view.Products.TrimEnd('\n');
                if (view.Quantity.Length > 0) view.Quantity = view.Quantity.TrimEnd('\n');

                shipments.Add(view);
            }

            ShipmentsDataGrid.ItemsSource = shipments;
        }

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void ToProduct(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ProductPage());
        }

        private void ToReport(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ReportPage());
        }

        private void ToSearchShipment(object sender, RoutedEventArgs e)
        {
            string query = FindShipmentTextBox.Text.ToLower();
            var filtered = shipments.Where(s =>
                s.Client.ToLower().Contains(query) ||
                s.Products.ToLower().Contains(query)
            ).ToList();
            ShipmentsDataGrid.ItemsSource = filtered;
        }

        private void ToFilterShipment(object sender, RoutedEventArgs e)
        {
            ShipmentsDataGrid.ItemsSource = shipments;
        }

        private void ToMarkShipped(object sender, RoutedEventArgs e)
        {
            if (ShipmentsDataGrid.SelectedItem is ShipmentView selected)
            {
                var dateWindow = new ShippingDateWindow { Owner = Window.GetWindow(this) };
                if (dateWindow.ShowDialog() == true && dateWindow.SelectedDate.HasValue)
                {
                    var order = context.Orders.FirstOrDefault(o => o.IdOrder == selected.IdOrder);
                    if (order != null)
                    {
                        order.StatusOrder = "Отгружен";
                        order.DateShipping = DateOnly.FromDateTime(dateWindow.SelectedDate.Value);
                        context.SaveChanges();

                        selected.Status = "Отгружен";
                        selected.ShippingDate = dateWindow.SelectedDate.Value.ToString("dd.MM.yyyy");
                        ShipmentsDataGrid.Items.Refresh();
                    }
                }
            }
        }

        private void ToMarkNotShipped(object sender, RoutedEventArgs e)
        {
            if (ShipmentsDataGrid.SelectedItem is ShipmentView selected)
            {
                var commentWindow = new CommentWindow { Owner = Window.GetWindow(this) };
                if (commentWindow.ShowDialog() == true)
                {
                    var order = context.Orders.FirstOrDefault(o => o.IdOrder == selected.IdOrder);
                    if (order != null)
                    {
                        order.StatusOrder = "Не отгружен";
                        order.CommentOrder = commentWindow.Comment;
                        context.SaveChanges();

                        selected.Status = "Не отгружен";
                        selected.Comment = commentWindow.Comment;
                        ShipmentsDataGrid.Items.Refresh();
                    }
                }
            }
        }
    }
}
