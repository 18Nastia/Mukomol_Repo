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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mukomol_Praktik.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddOrderPage.xaml
    /// </summary>
    public class ProductView
    {
        public int idProduct { get; set; }
        public string type {  get; set; }
        public string name { get; set; }
        public string? amount { get; set; }
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
        public void ViewProducts()
        {
            foreach (var productAll in context.Pasta.ToList())
            {
                ProductView productsView = new ProductView();
                productsView.type = "Макароны";
                productsView.idProduct = productAll.IdPasta;
                productsView.name = $"{productAll.TypePasta} {productAll.Brand}";
                if (productAll.Packaging != null) productsView.name += $" {productAll.Packaging}г";
                bool existsInBasket = false;
                foreach (var product in ProductsViewBascet)
                {
                    if (product.idProduct == productsView.idProduct && product.type == productsView.type)
                    {
                        existsInBasket = true;
                        break;
                    }
                }
                if (!existsInBasket)
                {
                    ProductsView.Add(productsView);
                }
            }
            foreach (var productAll in context.Flours.ToList())
            {
                ProductView productsView = new ProductView();
                productsView.type = "Мука";
                productsView.name = productAll.NameFlour;
                bool existsInBasket = false;
                foreach (var product in ProductsViewBascet)
                {
                    if (product.name == productsView.name && product.type == productsView.type)
                    {
                        existsInBasket = true;
                        break;
                    }
                }
                if (!existsInBasket)
                {
                    ProductsView.Add(productsView);
                }
            }
            foreach(var partner in context.Partners.ToList())
            {
                partners.Add(partner.NameCompany);
            }
            BascetProductsDataGrid.ItemsSource = ProductsViewBascet;
            AllProductsDataGrid.ItemsSource = ProductsView;
            PartnerComboBox.ItemsSource = partners;
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
                if(addProduct.type.ToLower() == "мука")
                {
                    foreach(var flour in context.Flours.ToList())
                    {
                        if(addProduct.name == flour.NameFlour)
                        {
                            product.IdFlour = flour.IdFlour;
                            product.IdOrder = order.IdOrder;
                        }
                    }
                }
                if (addProduct.type.ToLower() == "макароны")
                {
                    foreach (var pasta in context.Pasta.ToList())
                    {
                        if (addProduct.idProduct == pasta.IdPasta)
                        {
                            product.IdPasta = pasta.IdPasta;
                            product.IdOrder = order.IdOrder;
                        }
                    }
                } 
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите товар!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                addNewProduct = false;
            }
            if(!string.IsNullOrEmpty(CountProductTextBox.Text) && int.TryParse(CountProductTextBox.Text, out int count))
            {
                product.Amount = count;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите количество!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                addNewProduct = false;
            }
            if (addNewProduct)
            {
                products.Add(product);
                
                addProduct.amount = CountProductTextBox.Text;
                if (addProduct.type.ToLower() == "макароны" && addProduct.name.Substring(addProduct.name.Length - 1, 1).ToLower() == "г")
                {
                    addProduct.amount += " шт";
                }
                else addProduct.amount += " г";
                ProductsView.Remove(addProduct);
                ProductsViewBascet.Add(addProduct);

                AllProductsDataGrid.ItemsSource = null;
                AllProductsDataGrid.ItemsSource = ProductsView;

                BascetProductsDataGrid.ItemsSource = null;
                BascetProductsDataGrid.ItemsSource = ProductsViewBascet;

                CountProductTextBox.Text = "";

            }
        }

        private void SaveOrder(object sender, RoutedEventArgs e)
        {
            order.DateOrder = DateOnly.FromDateTime(DateTime.Now);
            order.StatusOrder = "Принят";
            bool addOrder = true;
            string partner = PartnerComboBox.Text;
            foreach (var partnerOfAll in context.Partners.ToList())
            {
                if(partnerOfAll.NameCompany == partner)
                {
                    order.IdPartner = partnerOfAll.IdPartner;
                    order.IdPartnerNavigation = partnerOfAll;
                }
            }
            if(string.IsNullOrEmpty(partner))
            {
                MessageBox.Show("Пожалуйста, выберите партнера!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                addOrder = false;
            }
            if(ProductsViewBascet.Count() == 0)
            {
                MessageBox.Show("Пожалуйста, выберите товары!",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                addOrder = false;
            }
            if (addOrder)
            {                
                context.Orders.Add(order);
                context.SaveChanges();
                foreach (var product in products)
                {
                    product.IdOrder = order.IdOrder;
                    context.Products.Add(product);
                }
                context.SaveChanges();
                NavigationService.Navigate(new Uri("/Pages/OrderPage.xaml", UriKind.Relative));
            }
        }
    }
}
