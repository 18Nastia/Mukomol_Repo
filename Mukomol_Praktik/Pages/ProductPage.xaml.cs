using Mukomol_Praktik.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Mukomol_Praktik.Pages
{
    public partial class ProductPage : Page
    {
        private List<ProductView> AllProducts = new();
        public ObservableCollection<ProductView> FilteredProducts { get; set; } = new();

        public ProductPage()
        {
            InitializeComponent();
            DataContext = this;

            LoadData();
            Loaded += (s, e) => ApplyFilter();
        }

        private void LoadData()
        {
            var context = new MukomolContext();
            AllProducts.Clear();

            // Мука
            foreach (var flour in context.Flours)
            {
                AllProducts.Add(new ProductView
                {
                    Name = flour.NameFlour,
                    Gost = flour.Gost,
                    Gluten = flour.Gluten,
                    White = flour.White,
                    Brand = null,
                    Packaging = null,
                    ImagePath = null,
                    Type = "Flour"
                });
            }

            // Макароны
            foreach (var pasta in context.Pasta)
            {
                bool isBulk = string.IsNullOrEmpty(pasta.Brand);

                AllProducts.Add(new ProductView
                {
                    Name = pasta.TypePasta,
                    Brand = pasta.Brand,
                    Packaging = pasta.Packaging,
                    ImagePath = pasta.ImagePasta?.StartsWith("/") == true
                                ? pasta.ImagePasta
                                : "/" + pasta.ImagePasta,

                    Type = isBulk ? "PastaBulk" : "PastaPackaged",

                    Gost = "-",
                    Gluten = "-",
                    White = "-"
                });
            }
        }

        private void ApplyFilter()
        {
            FilteredProducts.Clear();

            bool showFlour = FlourCheckBox?.IsChecked == true;
            bool showPackaged = PackagedPastaCheckBox?.IsChecked == true;
            bool showBulk = BulkPastaCheckBox?.IsChecked == true;

            foreach (var item in AllProducts)
            {
                if (item.Type == "Flour" && showFlour)
                    FilteredProducts.Add(item);

                if (item.Type == "PastaPackaged" && showPackaged)
                    FilteredProducts.Add(item);

                if (item.Type == "PastaBulk" && showBulk)
                    FilteredProducts.Add(item);
            }
        }

        private void FilterChanged(object sender, RoutedEventArgs e) => ApplyFilter();

        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            if (NavigationService != null && NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                // Можно, например, вывести сообщение или просто ничего не делать
                MessageBox.Show("Нет предыдущей страницы для возврата.", "Навигация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ToReport(object sender, MouseButtonEventArgs e) => NavigationService.Navigate(new ReportPage());
        private void ToShipment(object sender, MouseButtonEventArgs e) => NavigationService.Navigate(new ShipmentPage());
    }

    // Выбор шаблона
    public class ProductTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FlourTemplate { get; set; }
        public DataTemplate PastaTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ProductView product)
            {
                return product.Type == "Flour"
                    ? FlourTemplate
                    : PastaTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }

    // Конвертер для скрытия веса
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visible && visible == Visibility.Visible;
        }
    }
}
