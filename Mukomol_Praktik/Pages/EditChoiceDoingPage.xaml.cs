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
    public class ProductOrder : ProductView
    {
        public int idProductOrder { get; set; }
    }

    public partial class EditChoiceDoingPage : Page
    {
        private int orderId;
        private MukomolContext context;
        private List<Product> editProducts { get; set; }
        private List<Product> productsToDelete { get; set; }
        private List<ProductOrder> ProductsView { get; set; }

        // Новый конструктор, который принимает список товаров
        public EditChoiceDoingPage(int orderId, List<Product> existingProducts = null)
        {
            InitializeComponent();
            context = new MukomolContext();
            this.orderId = orderId;

            ProductsView = new List<ProductOrder>();
            productsToDelete = new List<Product>();

            if (existingProducts != null)
            {
                editProducts = existingProducts;
            }
            else
            {
                LoadProductsFromDatabase();
            }

            LoadOrderInfo();

            EditStatusComboBox.Items.Add("Принят");
            EditStatusComboBox.Items.Add("В обработке");
            EditStatusComboBox.Items.Add("Готов к отправке");
            EditStatusComboBox.Items.Add("Отправлен");
            EditStatusComboBox.Items.Add("Отменен");

            ViewProductsInOrder();
        }

        // Старый конструктор для обновления списка после добавления товара
        public EditChoiceDoingPage(Order orderEdit) : this(orderEdit.IdOrder)
        {
            var order = context.Orders
                .Include(o => o.IdPartnerNavigation)
                .FirstOrDefault(o => o.IdOrder == orderId);

            if (order != null)
            {
                PartnerTextBlock.Text = order.IdPartnerNavigation?.NameCompany ?? "";
                DateTextBlock.Text = order.DateOrder.ToString();
                StatusTextBlock.Text = order.StatusOrder;
            }
        }

        private void LoadProductsFromDatabase()
        {
            editProducts = context.Products
                .Include(p => p.IdPastaNavigation)
                .Include(p => p.IdFlourNavigation)
                .Where(p => p.IdOrder == orderId)
                .ToList();
        }

        private void LoadOrderInfo()
        {
            var order = context.Orders
                .Include(o => o.IdPartnerNavigation)
                .FirstOrDefault(o => o.IdOrder == orderId);

            if (order != null)
            {
                PartnerTextBlock.Text = order.IdPartnerNavigation?.NameCompany ?? "";
                DateTextBlock.Text = order.DateOrder.ToString();
                StatusTextBlock.Text = order.StatusOrder;
            }
        }

        private void ViewProductsInOrder()
        {
            EditAmountProductTextBox.Text = "";
            EditStatusComboBox.Text = "";
            ProductsView.Clear();

            foreach (var product in editProducts)
            {
                ProductOrder productsView = new ProductOrder();
                productsView.idProductOrder = product.IdProduct;

                if (product.IdPasta != null)
                {
                    var pasta = context.Pasta.Find(product.IdPasta);
                    if (pasta != null)
                    {
                        productsView.type = "Макароны";
                        productsView.idProduct = pasta.IdPasta;
                        productsView.name = $"{pasta.TypePasta} {pasta.Brand}";

                        if (pasta.Packaging != null)
                        {
                            productsView.name += $" {pasta.Packaging}г";
                            productsView.amount = $"{product.Amount} шт";
                        }
                        else
                        {
                            productsView.amount = $"{product.Amount} г";
                        }

                        ProductsView.Add(productsView);
                    }
                }
                else if (product.IdFlour != null)
                {
                    var flour = context.Flours.Find(product.IdFlour);
                    if (flour != null)
                    {
                        productsView.type = "Мука";
                        productsView.idProduct = flour.IdFlour;
                        productsView.name = flour.NameFlour;
                        productsView.amount = $"{product.Amount} г";

                        ProductsView.Add(productsView);
                    }
                }
            }

            ProductsInOrderDataGrid.ItemsSource = null;
            ProductsInOrderDataGrid.ItemsSource = ProductsView;
        }

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DeleteProductOrder(object sender, RoutedEventArgs e)
        {
            var deleteProduct = ProductsInOrderDataGrid.SelectedItem as ProductOrder;

            if (deleteProduct != null)
            {
                Product productToDelete = editProducts.FirstOrDefault(p => p.IdProduct == deleteProduct.idProductOrder);

                if (productToDelete != null)
                {
                    MessageBoxResult result = MessageBox.Show("Удалить товар?",
                                                      "Подтверждение",
                                                      MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        if (productToDelete.IdProduct > 0)
                        {
                            productsToDelete.Add(productToDelete);
                        }
                        editProducts.Remove(productToDelete);
                        ViewProductsInOrder();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления!");
            }
        }

        private void EditStatus(object sender, RoutedEventArgs e)
        {
            string newStatus = EditStatusComboBox.Text;

            if (!string.IsNullOrEmpty(newStatus))
            {
                var orderInDb = context.Orders.Find(orderId);
                if (orderInDb != null)
                {
                    orderInDb.StatusOrder = newStatus;
                    context.SaveChanges();
                    StatusTextBlock.Text = newStatus;
                    MessageBox.Show("Статус изменен!");
                }
            }
            else
            {
                MessageBox.Show("Выберите новый статус!");
            }
        }

        private void EditCountProduct(object sender, RoutedEventArgs e)
        {
            var editProduct = ProductsInOrderDataGrid.SelectedItem as ProductOrder;
            string newCount = EditAmountProductTextBox.Text;

            if (!string.IsNullOrEmpty(newCount) && editProduct != null && int.TryParse(newCount, out int countProduct) && countProduct > 0)
            {
                Product productToEdit = editProducts.FirstOrDefault(p => p.IdProduct == editProduct.idProductOrder);

                if (productToEdit != null)
                {
                    productToEdit.Amount = countProduct;
                    ViewProductsInOrder();
                    MessageBox.Show("Количество изменено!");
                }
            }
            else
            {
                MessageBox.Show("Введите корректное количество!");
            }
        }

        private void SaveEditOrder(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var product in editProducts.Where(p => p.IdProduct > 0))
                {
                    var productInDb = context.Products.Find(product.IdProduct);
                    if (productInDb != null && productInDb.Amount != product.Amount)
                    {
                        productInDb.Amount = product.Amount;
                        var orderInDb = context.Orders.Find(orderId);
                        if (orderInDb != null)
                        {
                            orderInDb.DateOrder = DateOnly.FromDateTime(DateTime.Now);
                        }
                    }
                    else if (productInDb != null)
                    {
                        productInDb.Amount = product.Amount;
                    }
                }

                foreach (var product in editProducts.Where(p => p.IdProduct == 0))
                {
                    context.Products.Add(product);
                    var orderInDb = context.Orders.Find(orderId);
                    if (orderInDb != null)
                    {
                        orderInDb.DateOrder = DateOnly.FromDateTime(DateTime.Now);
                    }
                }

                foreach (var productToDelete in productsToDelete)
                {
                    var productInDb = context.Products.Find(productToDelete.IdProduct);
                    if (productInDb != null)
                    {
                        context.Products.Remove(productInDb);
                        var orderInDb = context.Orders.Find(orderId);
                        if (orderInDb != null)
                        {
                            orderInDb.DateOrder = DateOnly.FromDateTime(DateTime.Now);
                        }
                    }
                }

                context.SaveChanges();

                productsToDelete.Clear();

                MessageBox.Show("Изменения сохранены!");
                NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void CancelEdit(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
        }

        private void ToAddProduct(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductOrderPage(orderId, editProducts));
        }
    }
}