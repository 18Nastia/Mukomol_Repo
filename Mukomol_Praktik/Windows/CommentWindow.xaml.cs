using System.Windows;

namespace Mukomol_Praktik.Pages
{
    public partial class CommentWindow : Window
    {
        public string Comment { get; private set; } = "";

        public CommentWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Comment = CommentTextBox.Text;
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
