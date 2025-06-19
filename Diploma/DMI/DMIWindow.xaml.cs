using System.Windows;

namespace Diploma
{
    public partial class DMIWindow : Window
    {
        public DMIWindow()
        {
            InitializeComponent();
            ContentArea.Content = new DMIInfoPage();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new DMIInfoPage();
        }

        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new DMISimulationPage();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}