using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Mukomol_Praktik.Models;
using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
    /// Логика взаимодействия для AddPartnerPage.xaml
    /// </summary>
    public partial class AddPartnerPage : Page
    {
        private Partner partner {  get; set; }
        public string fileName { get; set; }
        public bool isEdit;

        private MukomolContext context;
        public AddPartnerPage(Partner? partnerEdit)
        {
            InitializeComponent();
            context = new MukomolContext();
            partner = new Partner();
            DropArea.AllowDrop = true;
            isEdit = false;
            DropArea.PreviewDragOver += DropArea_PreviewDragOver;
            if (partnerEdit != null )
            {
                isEdit = true;
                namePartnerPage.Text = "Изменение партнёра";
                partner = context.Partners.Find(partnerEdit.IdPartner);
                nameCompanyTextBox.Text = partner.NameCompany;
                numberTextBox.Text = partner.ContactNumber;
            }
            else isEdit = false;
        }
        private void DropArea_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void ToBack(object sender, MouseButtonEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AddPartner(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(nameCompanyTextBox.Text))
            {
                partner.NameCompany = nameCompanyTextBox.Text;
            }
            else {
                MessageBox.Show("Пожалуйста, ведите название компании, это обязательное для ввода поле",
                                "Не заполнено обязательное поле",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            if (!string.IsNullOrEmpty(fileName))
            {
                partner.LogoPartner = $"/Image/image_logo/{fileName}";
            }
            else if(!isEdit)
            {
                MessageBox.Show("Пожалуйста, загрузьте логотип компании",
                                "Не заполнено обязательное поле",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            if (!string.IsNullOrEmpty(numberTextBox.Text) && Int64.TryParse(numberTextBox.Text, out Int64 number) && numberTextBox.Text.Length == 11)
            {
                partner.ContactNumber = numberTextBox.Text;
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите контактный номер компании",
                                "Не заполнено обязательное поле",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            if (!isEdit)
            {
                context.Partners.Add(partner);
            }
            context.SaveChanges();
            NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
            if (!isEdit)
                MessageBox.Show("Партнёр успешно добавлен!",
                            "Оповещение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            else
                MessageBox.Show("Информация о партнёре изменена успешно!",
                            "Оповещение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }
        private void DropImage(object sender, DragEventArgs e)
        {
            DropArea.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(255, 245, 245, 245));

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string filePath = files[0];
                    LoadImage(filePath);
                }
            }
        }
        private void ChoiceFileButton(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg; *.jpeg; *.png; *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Все файлы (*.*)|*.*",
                Title = "Выберите изображение"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                LoadImage(openFileDialog.FileName);
            }

        }
        private void LoadImage(string filePath)
        {
            try
            {
                bool trueExtension = false;
                fileName = System.IO.Path.GetFileName(filePath);
                string extension = System.IO.Path.GetExtension(filePath).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                foreach (string allowedExtension in allowedExtensions)
                {
                    if (allowedExtension == extension) trueExtension = true;
                }
                if (!trueExtension)
                {
                    MessageBox.Show("Пожалуйста, выберите файл изображения (JPG, PNG, BMP, GIF)",
                                    "Неверный формат",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                string destinationPath = System.IO.Path.Combine("../../../Image/image_logo", fileName);

                File.Copy(filePath, destinationPath, true);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                AddFileToCsproj(fileName);
                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;

                ((TextBlock)((StackPanel)DropArea.Children[0]).Children[0]).Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
        //Метод для превращения изображения в ресурс при сборке
        private void AddFileToCsproj(string fileName)
        {
            try
            {
                //Получение пути до корневой директории проекта
                string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
                //Получение пути до файла .csproj проекта
                string csprojPath = Directory.GetFiles(projectRoot, "*.csproj").FirstOrDefault();

                if (csprojPath != null)
                {                    
                    string csprojContent = File.ReadAllText(csprojPath);
                    string resourceEntry =
                        $"\n    <Resource Include=\"Image\\image_logo\\{fileName}\">\n" +
                        "      <CopyToOutputDirectory>Never</CopyToOutputDirectory>\n" +
                        "    </Resource>";
                    string resourceEntry2 =
                        $"\n    <None Remove=\"Image\\image_logo\\{fileName}\"/>";
                    if (csprojContent.Contains(resourceEntry) || csprojContent.Contains(resourceEntry2))
                    {
                        return;
                    }
                    int lastItemGroup = csprojContent.LastIndexOf("<ItemGroup>");
                    if (lastItemGroup > 0)
                    {
                        csprojContent = csprojContent.Insert(lastItemGroup + "<ItemGroup>".Length, resourceEntry);
                        File.WriteAllText(csprojPath, csprojContent);

                        Console.WriteLine($"Файл добавлен в .csproj: {fileName}");
                    }
                    int firstItemGroup = csprojContent.IndexOf("<ItemGroup>");
                    if (lastItemGroup > 0)
                    {
                        csprojContent = csprojContent.Insert(firstItemGroup + "<ItemGroup>".Length, resourceEntry2);
                        File.WriteAllText(csprojPath, csprojContent);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось добавить в .csproj: {ex.Message}");
            }
        }

        private void CancelAddPartner(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Еслы вы отмените, то данные не будут сохранены. Вы уверены, что хотите отменить добавление партнера?",
                                                      "Подтверждение",
                                                      MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                NavigationService.Navigate(new Uri("/Pages/PartnerPage.xaml", UriKind.Relative));
            }
            else
            {
                return;
            }
        }
    }
}
