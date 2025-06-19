using System.Windows;
using System.Windows.Controls;

namespace Diploma
{
    public partial class SouthBridgeSimulationPage : UserControl
    {
        public SouthBridgeSimulationPage()
        {
            InitializeComponent();
        }

        private void PCIeButton_Click(object sender, RoutedEventArgs e)
        {
            var pcieWindow = new PCIeWindow();
            pcieWindow.Show();

            // force switch to simulation page
            if (pcieWindow.ContentArea != null)
            {
                pcieWindow.ContentArea.Content = new SimulationPage();
            }
        }
        private void nbButton_Click(object sender, RoutedEventArgs e)
        {
            var nbWindow = new NorthBridgeWindow();
            nbWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }
        
        private void SATAButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to new SATA simulation
            var sataWindow = new SataWindow();
            sataWindow.Show();
        }

        private void USBButton_Click(object sender, RoutedEventArgs e)
        {
            var usbWindow = new UsbWindow();
            usbWindow.Show();
        }


    }
}
