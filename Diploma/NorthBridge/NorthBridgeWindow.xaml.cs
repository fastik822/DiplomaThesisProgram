using System.Windows;

namespace Diploma
{
    public partial class NorthBridgeWindow : Window
    {
        public NorthBridgeWindow()
        {
            InitializeComponent();
            ContentArea.Content = new NorthBridgeInfoPage();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new NorthBridgeInfoPage();
        }

        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new NorthBridgeSimulationPage();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
