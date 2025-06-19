using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diploma
{
    public partial class UsbWindow : Window
    {
        public UsbWindow()
        {
            InitializeComponent();
            Log("USB 2.0 Bus ready – 480 Mbit/s");
        }

        private async void IN_Click(object sender, RoutedEventArgs e)
        {
            Log("Host → IN Token");
            await AnimatePacket("IN", Colors.Yellow, reverse: false);

            Log("Device → Data");
            await AnimatePacket("DATA", Colors.LightBlue, reverse: true);

            Log("Host → ACK");
            await AnimatePacket("ACK", Colors.LightGreen, reverse: false);
        }


        private async void OUT_Click(object sender, RoutedEventArgs e)
        {
            Log("Host → OUT Token");
            await AnimatePacket("OUT", Colors.Orange, reverse: false);

            Log("Host → DATA");
            await AnimatePacket("DATA", Colors.LightBlue, reverse: false);

            Log("Device → NAK");
            await AnimatePacket("NAK", Colors.Red, reverse: true);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            UsbLog.Clear();
            Log("Log cleared.");
        }

        private async Task AnimatePacket(string label, Color fill, bool reverse = false)
        {
            double fromX = reverse ? 330 : 170;
            double toX = reverse ? 170 : 330;
            double y = 180;

            UsbPacket.Fill = new SolidColorBrush(fill);
            UsbPacketLabel.Text = label;
            UsbPacket.Visibility = Visibility.Visible;
            UsbPacketLabel.Visibility = Visibility.Visible;

            Canvas.SetLeft(UsbPacket, fromX);
            Canvas.SetTop(UsbPacket, y);
            Canvas.SetLeft(UsbPacketLabel, fromX);
            Canvas.SetTop(UsbPacketLabel, y - 20);

            var duration = TimeSpan.FromSeconds(0.8);
            var animX = new DoubleAnimation(fromX, toX, duration);
            var labelX = new DoubleAnimation(fromX, toX, duration);

            TaskCompletionSource<bool> tcs = new();
            animX.Completed += (_, __) =>
            {
                UsbPacket.Visibility = Visibility.Hidden;
                UsbPacketLabel.Visibility = Visibility.Hidden;
                tcs.SetResult(true);
            };

            UsbPacket.BeginAnimation(Canvas.LeftProperty, animX);
            UsbPacketLabel.BeginAnimation(Canvas.LeftProperty, labelX);
            await tcs.Task;
            await Task.Delay(200);
        }

        private void Log(string message)
        {
            UsbLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
            UsbLog.ScrollToEnd();
        }
    }
}
