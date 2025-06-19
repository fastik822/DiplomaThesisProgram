using System.Windows;

namespace Diploma
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PCIeButton_Click(object sender, RoutedEventArgs e)
        {
            var pcieWindow = new PCIeWindow();
            pcieWindow.Show();
            this.Close();
        }

        private void DMIButton_Click(object sender, RoutedEventArgs e)
        {
            var dmiWindow = new DMIWindow();
            dmiWindow.Show();
            this.Close();
        }

        private void SouthBridgeButton_Click(object sender, RoutedEventArgs e)
        {
            var sbWindow = new SouthBridgeWindow();
            sbWindow.Show();
            this.Close();
        }

        private void NorthBridgeButton_Click(object sender, RoutedEventArgs e)
        {
            var nbWindow = new NorthBridgeWindow();
            nbWindow.Show();
            this.Close();
        }
    }
}