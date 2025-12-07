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

namespace Mukomol_Praktik.Pages
{
    /// <summary>
    /// Логика взаимодействия для ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        private MukomolContext context;

        public ReportPage()
        {
            InitializeComponent();
            context = new MukomolContext();
        }

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

        private void ToProduct(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ProductPage());
        }

        private void ToShipment(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ShipmentPage());
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите даты начала и конца периода.");
                return;
            }

            DateOnly startDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value);
            DateOnly endDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value);

            var orders = context.Orders.Include(o => o.IdPartnerNavigation)
                                       .Include(o => o.Products)
                                       .ThenInclude(p => p.IdFlourNavigation)
                                       .Include(o => o.Products)
                                       .ThenInclude(p => p.IdPastaNavigation)
                                       .ToList();

            var filteredOrders = orders.Where(o => o.DateOrder >= startDate && o.DateOrder <= endDate).ToList();

            // Всего заказов
            TotalOrdersText.Text = $"Всего заказов: {filteredOrders.Count}";

            // Отгружено
            ShippedOrdersText.Text = $"Отгружено заказов: {filteredOrders.Count(o => o.StatusOrder == "Отгружен")}";

            // Топ клиентов
            var topClients = filteredOrders.GroupBy(o => o.IdPartnerNavigation?.NameCompany)
                                           .Select(g => new { Client = g.Key, Count = g.Count() })
                                           .OrderByDescending(x => x.Count)
                                           .Take(5);
            TopClientsText.Text = string.Join("\n", topClients.Select(x => $"{x.Client} — {x.Count}"));

            // Топ товаров
            var topProducts = filteredOrders.SelectMany(o => o.Products)
                                            .Select(p => p.IdFlourNavigation?.NameFlour ?? p.IdPastaNavigation?.TypePasta)
                                            .GroupBy(name => name)
                                            .Select(g => new { Product = g.Key, Count = g.Count() })
                                            .OrderByDescending(x => x.Count)
                                            .Take(5);
            TopProductsText.Text = string.Join("\n", topProducts.Select(x => $"{x.Product} — {x.Count}"));
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период перед экспортом.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = StartDatePicker.SelectedDate.Value;
            DateTime endDate = EndDatePicker.SelectedDate.Value;

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("Отчет");

                    // Заголовок
                    ws.Cell(1, 1).Value = $"ОТЧЕТ ЗА ПЕРИОД С {startDate:dd.MM.yyyy} ПО {endDate:dd.MM.yyyy}";
                    ws.Range(1, 1, 1, 2).Merge().Style.Font.SetBold().Font.FontSize = 16;
                    ws.Range(1, 1, 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    // Шапки таблиц
                    ws.Cell(3, 1).Value = "Топ клиентов";
                    ws.Cell(3, 2).Value = "Топ товаров";
                    ws.Range(3, 1, 3, 2).Style.Font.SetBold().Font.FontSize = 14;
                    ws.Range(3, 1, 3, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    ws.Range(3, 1, 3, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    // Данные
                    var clients = TopClientsText.Text.Split('\n');
                    var products = TopProductsText.Text.Split('\n');
                    int maxRows = Math.Max(clients.Length, products.Length);

                    for (int i = 0; i < maxRows; i++)
                    {
                        ws.Cell(4 + i, 1).Value = i < clients.Length ? clients[i] : "";
                        ws.Cell(4 + i, 2).Value = i < products.Length ? products[i] : "";
                    }

                    // Итоги
                    int summaryRow = 4 + maxRows + 1;
                    ws.Cell(summaryRow, 1).Value = TotalOrdersText.Text;
                    ws.Cell(summaryRow + 1, 1).Value = ShippedOrdersText.Text;
                    ws.Range(summaryRow, 1, summaryRow + 1, 2).Style.Font.SetBold().Font.FontSize = 14;
                    ws.Range(summaryRow, 1, summaryRow + 1, 2).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    ws.Range(summaryRow, 1, summaryRow + 1, 2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    // Форматирование столбцов
                    ws.Columns().AdjustToContents();

                    // Сохранение
                    var saveDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Workbook|*.xlsx",
                        FileName = $"Отчет_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx"
                    };
                    if (saveDialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(saveDialog.FileName);
                        MessageBox.Show("Отчет успешно экспортирован.", "Excel", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
