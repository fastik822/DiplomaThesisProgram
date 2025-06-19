using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diploma
{
    public partial class SataWindow : Window
    {
        public SataWindow()
        {
            InitializeComponent();
            Log("SATA 3.0 Interface Ready – 6.0 Gbit/s");
        }

        private async void RegisterFIS_Click(object sender, RoutedEventArgs e)
        {
            await AnimatePacket("Register FIS", Colors.Gold);
            Log("Sent: Register FIS → device");
            await Task.Delay(400);
            await AnimatePacket("Status", Colors.Gold, reverse: true);
            Log("Received: Device Status ←");
        }

        private async void DataFIS_Click(object sender, RoutedEventArgs e)
        {
            await AnimatePacket("Data FIS", Colors.LightBlue);
            Log("Sent: Data FIS → device");
        }

        private async void SetBitsFIS_Click(object sender, RoutedEventArgs e)
        {
            await AnimatePacket("Set Dev Bits", Colors.OrangeRed);
            Log("Sent: Set Device Bits FIS → device");
        }

        private async Task AnimatePacket(string label, Color color, bool reverse = false)
        {
            double fromX = reverse ? 385 : 155;
            double toX = reverse ? 155 : 385;
            double y = 180;

            Packet.Fill = new SolidColorBrush(color);
            PacketLabel.Text = label;
            Packet.Visibility = Visibility.Visible;
            PacketLabel.Visibility = Visibility.Visible;

            Canvas.SetLeft(Packet, fromX);
            Canvas.SetTop(Packet, y);
            Canvas.SetLeft(PacketLabel, fromX);
            Canvas.SetTop(PacketLabel, y - 20);

            var duration = TimeSpan.FromSeconds(0.8);
            var animX = new DoubleAnimation(fromX, toX, duration);
            var labelX = new DoubleAnimation(fromX, toX, duration);

            TaskCompletionSource<bool> tcs = new();
            animX.Completed += (_, __) =>
            {
                Packet.Visibility = Visibility.Hidden;
                PacketLabel.Visibility = Visibility.Hidden;
                tcs.SetResult(true);
            };

            Packet.BeginAnimation(Canvas.LeftProperty, animX);
            PacketLabel.BeginAnimation(Canvas.LeftProperty, labelX);
            await tcs.Task;
            await Task.Delay(200);
        }

        private void Log(string message)
        {
            StatusLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
            StatusLog.ScrollToEnd();
        }
    }
}
