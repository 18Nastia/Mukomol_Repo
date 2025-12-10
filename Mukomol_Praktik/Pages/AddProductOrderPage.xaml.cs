using Mukomol_Praktik.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mukomol_Praktik.Pages
{
    public partial class AddProductOrderPage : Page
    {
        private MukomolContext context;
        private int orderId;
        private List<Product> currentOrderProducts;

        public AddProductOrderPage(int orderId, List<Product> existingProducts)
        {
            InitializeComponent();
            context = new MukomolContext();
            this.orderId = orderId;

            currentOrderProducts = existingProducts ?? new List<Product>();

            ViewAddProducts();
        }

        private void ViewAddProducts()
        {
            var availableProducts = new List<ProductView>();

            foreach (var pasta in context.Pasta.ToList())
            {
                bool alreadyInOrder = currentOrderProducts.Any(p => p.IdPasta == pasta.IdPasta);

                if (!alreadyInOrder)
                {
                    ProductView productView = new ProductView();
                    productView.type = "Макароны";
                    productView.idProduct = pasta.IdPasta;
                    productView.name = $"{pasta.TypePasta} {pasta.Brand}";
                    if (pasta.Packaging != null)
                        productView.name += $" {pasta.Packaging}г";

                    availableProducts.Add(productView);
                }
            }

            foreach (var flour in context.Flours.ToList())
            {
                bool alreadyInOrder = currentOrderProducts.Any(p => p.IdFlour == flour.IdFlour);

                if (!alreadyInOrder)
                {
                    ProductView productView = new ProductView();
                    productView.type = "Мука";
                    productView.idProduct = flour.IdFlour;
                    productView.name = flour.NameFlour;

                    availableProducts.Add(productView);
                }
            }

            ProductsDataGrid.ItemsSource = availableProducts;
        }

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddProduct(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsDataGrid.SelectedItem as ProductView;

            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар!");
                return;
            }

            if (!string.IsNullOrEmpty(amountNewProduct.Text) &&
                int.TryParse(amountNewProduct.Text, out int amount) && amount > 0)
            {
                Product newProduct = new Product
                {
                    IdOrder = orderId,
                    Amount = amount
                };

                if (selectedProduct.type == "Макароны")
                {
                    newProduct.IdPasta = selectedProduct.idProduct;
                }
                else if (selectedProduct.type == "Мука")
                {
                    newProduct.IdFlour = selectedProduct.idProduct;
                }

                // Создание новой страницы с обновленным списком товаров
                var updatedProducts = new List<Product>(currentOrderProducts)
                {
                    newProduct
                };

                NavigationService.Navigate(new EditChoiceDoingPage(orderId, updatedProducts));
            }
            else
            {
                MessageBox.Show("Введите корректное количество!");
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}