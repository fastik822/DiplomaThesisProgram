using System.Windows.Controls;
using System.Windows.Input;

namespace Diploma
{
    public partial class SouthBridgeInfoPage : UserControl
    {
        public SouthBridgeInfoPage()
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
