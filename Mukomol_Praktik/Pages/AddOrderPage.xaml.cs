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
    /// <summary>
    /// Логика взаимодействия для AddOrderPage.xaml
    /// </summary>
    public class ProductView
    {
        public int idProduct { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string? amount { get; set; }
        public int originalAmount { get; set; }
    }

    public partial class AddOrderPage : Page
    {
        private MukomolContext context;
        private List<ProductView> ProductsViewBascet { get; set; }
        private List<ProductView> ProductsView { get; set; }
        private List<Product> products { get; set; }
        private List<string> partners { get; set; }
        public Order order { get; set; }

        public AddOrderPage()
        {
            InitializeComponent();
            context = new MukomolContext();
            ProductsView = new List<ProductView>();
            ProductsViewBascet = new List<ProductView>();
            products = new List<Product>();
            partners = new List<string>();
            order = new Order();
            ViewProducts();
        }

        //конструктор для повторения заказа
        public AddOrderPage(List<ProductView> repeatedProducts, string partnerName = "") : this()
        {
            if (!string.IsNullOrEmpty(partnerName))
            {
                PartnerComboBox.SelectedItem = partnerName;
            }

            foreach (var productView in repeatedProducts)
            {
                AddProductToBasket(productView);
            }
        }


        private void AddProductToBasket(ProductView productView)
        {
            Product product = new Product();
            product.Amount = productView.originalAmount;

            if (productView.type.ToLower() == "мука")
            {
                var flour = context.Flours.FirstOrDefault(f => f.NameFlour == productView.name);
                if (flour != null)
                {
                    product.IdFlour = flour.IdFlour;
                }
            }
            else if (productView.type.ToLower() == "макароны")
            {
                product.IdPasta = productView.idProduct;
            }

            products.Add(product);
            ProductsViewBascet.Add(productView);

            //убираем из общего списка
            var productToRemove = ProductsView.FirstOrDefault(p =>
                (p.type == productView.type && p.name == productView.name) ||
                (p.type == productView.type && p.idProduct == productView.idProduct));

            if (productToRemove != null)
            {
                ProductsView.Remove(productToRemove);
            }

            RefreshDataGrids();
        }

        private void UpdateAvailableProducts()
        {
            //обновление общего списка
            ProductsView.Clear();

            foreach (var pasta in context.Pasta.ToList())
            {
                if (!ProductsViewBascet.Any(p => p.type == "Макароны" && p.idProduct == pasta.IdPasta))
                {
                    ProductView productView = new ProductView
                    {
                        type = "Макароны",
                        idProduct = pasta.IdPasta,
                        name = $"{pasta.TypePasta} {pasta.Brand}"
                    };
                    if (pasta.Packaging != null)
                        productView.name += $" {pasta.Packaging}г";
                    ProductsView.Add(productView);
                }
            }

            foreach (var flour in context.Flours.ToList())
            {
                if (!ProductsViewBascet.Any(p => p.type == "Мука" && p.name == flour.NameFlour))
                {
                    ProductView productView = new ProductView
                    {
                        type = "Мука",
                        name = flour.NameFlour
                    };
                    ProductsView.Add(productView);
                }
            }
        }

        private void RefreshDataGrids()
        {
            AllProductsDataGrid.ItemsSource = null;
            AllProductsDataGrid.ItemsSource = ProductsView;
            BascetProductsDataGrid.ItemsSource = null;
            BascetProductsDataGrid.ItemsSource = ProductsViewBascet;
        }

        public void ViewProducts()
        {
            UpdateAvailableProducts();

            partners.Clear();
            foreach (var partner in context.Partners.ToList())
            {
                partners.Add(partner.NameCompany);
            }

            PartnerComboBox.ItemsSource = partners;
            RefreshDataGrids();
        }

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddProductToOrder(object sender, RoutedEventArgs e)
        {
            Product product = new Product();
            bool addNewProduct = true;
            var addProduct = AllProductsDataGrid.SelectedItem as ProductView;

            if (addProduct != null)
            {
                if (addProduct.type.ToLower() == "мука")
                {
                    var flour = context.Flours.FirstOrDefault(f => f.NameFlour == addProduct.name);
                    if (flour != null)
                    {
                        product.IdFlour = flour.IdFlour;
                        product.IdOrder = order.IdOrder;
                    }
                }
                else if (addProduct.type.ToLower() == "макароны")
                {
                    product.IdPasta = addProduct.idProduct;
                    product.IdOrder = order.IdOrder;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                addNewProduct = false;
            }

            if (!string.IsNullOrEmpty(CountProductTextBox.Text) && int.TryParse(CountProductTextBox.Text, out int count))
            {
                product.Amount = count;
                addProduct.originalAmount = count;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите количество!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                addNewProduct = false;
            }

            if (addNewProduct)
            {
                products.Add(product);

                addProduct.amount = CountProductTextBox.Text;
                if (addProduct.type.ToLower() == "макароны" && addProduct.name.Contains("г") && !addProduct.name.EndsWith(" шт"))
                {
                    addProduct.amount += " шт";
                }
                else
                {
                    addProduct.amount += " г";
                }

                ProductsView.Remove(addProduct);
                ProductsViewBascet.Add(addProduct);

                RefreshDataGrids();
                CountProductTextBox.Text = "";
            }
        }

        private void SaveOrder(object sender, RoutedEventArgs e)
        {
            order.DateOrder = DateOnly.FromDateTime(DateTime.Now);
            order.StatusOrder = "Принят";
            bool addOrder = true;
            string partner = PartnerComboBox.Text;

            var selectedPartner = context.Partners.FirstOrDefault(p => p.NameCompany == partner);
            if (selectedPartner != null)
            {
                order.IdPartner = selectedPartner.IdPartner;
                order.IdPartnerNavigation = selectedPartner;
            }

            if (string.IsNullOrEmpty(partner))
            {
                MessageBox.Show("Пожалуйста, выберите партнера!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                addOrder = false;
            }

            if (ProductsViewBascet.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите товары!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                addOrder = false;
            }

            if (addOrder)
            {
                try
                {
                    context.Orders.Add(order);
                    context.SaveChanges();

                    foreach (var product in products)
                    {
                        product.IdOrder = order.IdOrder;
                        context.Products.Add(product);
                    }

                    context.SaveChanges();
                    MessageBox.Show("Заказ успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RepeatOrder(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RepeatOrderPage.xaml", UriKind.Relative));
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void EditAmount(object sender, RoutedEventArgs e)
        {
            var editProduct = BascetProductsDataGrid.SelectedItem as ProductView;
            string newCount = CountProductTextBox.Text;

            if (!string.IsNullOrEmpty(newCount) && editProduct != null && int.TryParse(newCount, out int countProduct) && countProduct > 0)
            {
                Product productToEdit = products.FirstOrDefault(p =>
                    (editProduct.type.ToLower() == "мука" &&
                     context.Flours.FirstOrDefault(f => f.NameFlour == editProduct.name)?.IdFlour == p.IdFlour) ||
                    (editProduct.type.ToLower() == "макароны" &&
                     editProduct.idProduct == p.IdPasta));

                if (productToEdit != null)
                {
                    productToEdit.Amount = countProduct;
                    editProduct.originalAmount = countProduct;

                    if (editProduct.type.ToLower() == "макароны" && editProduct.name.Contains("г") && !editProduct.name.EndsWith(" шт"))
                    {
                        editProduct.amount = $"{countProduct} шт";
                    }
                    else
                    {
                        editProduct.amount = $"{countProduct} г";
                    }

                    RefreshDataGrids();
                    CountProductTextBox.Text = "";
                    MessageBox.Show("Количество изменено!");
                }
                else
                {
                    MessageBox.Show("Товар не найден в списке!");
                }
            }
            else
            {
                MessageBox.Show("Введите корректное количество!");
            }
        }
    }
}