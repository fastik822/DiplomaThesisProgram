using System.Windows;

namespace Diploma
{
    public partial class PCIeWindow : Window
    {
        public PCIeWindow()
        {
            InitializeComponent();
            ContentArea.Content = new InfoPage();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new InfoPage();
        }

        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SimulationPage();
        }
        private void SimX4Button_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new PCIeX4SimulationPage();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}