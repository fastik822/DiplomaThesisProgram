using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace Diploma
{
    public partial class NorthBridgeInfoPage : UserControl
    {
        public NorthBridgeInfoPage()
        {
            InitializeComponent();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;
            if (image != null && image.Source != null)
            {
                var previewWindow = new FullscreenImageWindow(image.Source);
                previewWindow.ShowDialog();
            }
        }
    }
}
