using System.Windows;
using System.Windows.Media;

namespace Diploma
{
    public partial class FullscreenImageWindow : Window
    {
        public FullscreenImageWindow(ImageSource source)
        {
            InitializeComponent();
            FullImage.Source = source;
        }

        private void Window_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
