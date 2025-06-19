using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diploma
{
    public partial class NorthBridgeSimulationPage : UserControl
    {
        private void sbButton_Click(object sender, RoutedEventArgs e)
        {
            var sbWindow = new SouthBridgeWindow();
            sbWindow.Show();

            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private int seq = 1;

        public NorthBridgeSimulationPage()
        {
            InitializeComponent();
            LogMsg("North Bridge simulation initialized.");
        }

        private async void InstructionFetch_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            LogMsg($"[Seq {seq}] CPU begins instruction fetch sequence (FSB).");

            LogMsg($"[Seq {seq}] Address Phase — Sending memory address to North Bridge.");
            await Animate(310, 375, 130, 130, "Address", Colors.SkyBlue);

            LogMsg($"[Seq {seq}] Control Phase — Read instruction request issued.");
            await Animate(310, 375, 130, 130, "Read Cmd", Colors.MediumPurple);

            LogMsg($"[Seq {seq}] Data Phase — Requesting 64B cache line.");
            await Animate(310, 375, 130, 130, "Request Data", Colors.Gold);

            await Task.Delay(400);
            LogMsg($"[Seq {seq}] North Bridge processes request...");

            await Task.Delay(500);
            LogMsg($"[Seq {seq}] Data returned to CPU.");
            await Animate(375, 310, 130, 130, "Instruction", Colors.LightGreen);

            seq++;
        }

        private async void RamBurstFetch_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            LogMsg($"[Seq {seq}] Northbridge activates DRAM row.");
            await Animate(505, 570, 130, 130, "Activate", Colors.DarkBlue);

            LogMsg($"[Seq {seq}] Sending column address (RAS/CAS).");
            await Animate(505, 570, 130, 130, "Address", Colors.LightBlue);

            LogMsg($"[Seq {seq}] Read command issued.");
            await Animate(505, 570, 130, 130, "Read Cmd", Colors.MediumPurple);

            LogMsg($"[Seq {seq}] Waiting on CAS latency...");
            await Task.Delay(500);

            LogMsg($"[Seq {seq}] 64-byte data burst returned to NB.");
            await Animate(570, 505, 130, 130, "64B Burst", Colors.Green);

            seq++;
        }

        private async void SataDmaRequest_Click(object sender, RoutedEventArgs e)
        {
            PktLabel.Foreground = Brushes.Red;
            Log.Clear();
            LogMsg($"[Seq {seq}] SATA device sends DMA request to Southbridge.");
            await Animate(420, 420, 280, 150, "DMA Req", Colors.OrangeRed, hideAfter: false);

            LogMsg($"[Seq {seq}] Southbridge forwards request to Northbridge.");
            await Animate(420, 420, 150, 130, "DMA Req", Colors.OrangeRed);

            PktLabel.Foreground = Brushes.Black;

            LogMsg($"[Seq {seq}] Northbridge writes 4KB data to RAM.");
            await Animate(505, 570, 130, 130, "4KB Write", Colors.MediumPurple);

            await Task.Delay(300);

            LogMsg($"[Seq {seq}] Northbridge acknowledges write.");
            await Animate(375, 310, 130, 130, "INT", Colors.LightBlue);

            LogMsg($"[Seq {seq}] DMA transfer complete.");
            seq++;
            
        }

        private async Task Animate(double fx, double tx, double fy, double ty, string label, Color color, bool hideAfter = true)
        {
            Pkt.Fill = new SolidColorBrush(color);
            PktLabel.Text = label;
            Pkt.Visibility = PktLabel.Visibility = Visibility.Visible;

            Canvas.SetLeft(Pkt, fx);
            Canvas.SetTop(Pkt, fy);
            Canvas.SetLeft(PktLabel, fx);
            Canvas.SetTop(PktLabel, fy - 20);

            TimeSpan dur = TimeSpan.FromSeconds(0.8);
            var ax = new DoubleAnimation(fx, tx, dur);
            var ay = new DoubleAnimation(fy, ty, dur);
            var lx = new DoubleAnimation(fx, tx, dur);
            var ly = new DoubleAnimation(fy - 20, ty - 20, dur);

            var tcs = new TaskCompletionSource<bool>();
            ax.Completed += (_, __) =>
            {
                if (hideAfter)
                {
                    Pkt.Visibility = Visibility.Hidden;
                    PktLabel.Visibility = Visibility.Hidden;
                }
                tcs.SetResult(true);
            };

            Pkt.BeginAnimation(Canvas.LeftProperty, ax);
            Pkt.BeginAnimation(Canvas.TopProperty, ay);
            PktLabel.BeginAnimation(Canvas.LeftProperty, lx);
            PktLabel.BeginAnimation(Canvas.TopProperty, ly);

            await tcs.Task;
            await Task.Delay(250);
        }


        private void LogMsg(string msg)
        {
            Log.AppendText($"{DateTime.Now:HH:mm:ss} — {msg}\n");
            Log.ScrollToEnd();
        }
    }
}
