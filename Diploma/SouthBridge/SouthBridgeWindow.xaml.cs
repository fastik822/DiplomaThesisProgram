using System.Windows;

namespace Diploma
{
    public partial class SouthBridgeWindow : Window
    {
        public SouthBridgeWindow()
        {
            InitializeComponent();
            ContentArea.Content = new SouthBridgeInfoPage();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SouthBridgeInfoPage();
        }

        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SouthBridgeSimulationPage();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
