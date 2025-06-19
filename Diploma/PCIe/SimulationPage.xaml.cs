using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diploma
{
    public partial class SimulationPage : UserControl
    {
        // link capabilities
        private const int LaneWidth = 1;         
        private const double Gtps = 8.0;
        private const double EffectiveGBs = 0.985;   
        private int flowControlCredits = 8;

        private int sequenceNumber = 1;
        private bool linkUp;
        private bool cpuToGpuLayersHighlighted = false; // Track CPU-to-GPU/Switch layers
        private bool gpuToCpuLayersHighlighted = false; // Track GPU-to-CPU/Switch layers


        public SimulationPage()
        {
            InitializeComponent();
            BandwidthLabel.Text = $"Bandwidth: {EffectiveGBs} GB/s per dir (PCIe 3.0 ×{LaneWidth})";
            CreditsIndicator.Text = $"Credits: {flowControlCredits}";
            UpdateStatusLog("Ready – link down.");
        }

        

        private async void Init_Click(object sender, RoutedEventArgs e)
        {
            if (linkUp) { UpdateStatusLog("Link already up."); return; }

            UpdateStatusLog("Physical: Detecting partner…");
            await Task.Delay(350);
            UpdateStatusLog($"Physical: Negotiating lane width (×{LaneWidth})");
            await Task.Delay(350);
            UpdateStatusLog($"Physical: Setting speed ({Gtps} GT/s per lane)");
            await Task.Delay(550);

            linkUp = true;
            UpdateStatusLog($"Link training complete – {EffectiveGBs} GB/s each direction.");
            StartTransferButton.IsEnabled = IntroduceErrorButton.IsEnabled = MemoryWriteButton.IsEnabled = true;
        }

        private async void StartTransfer_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            cpuToGpuLayersHighlighted = false; // Reset for new transaction
            gpuToCpuLayersHighlighted = false; // Reset for new transaction
            if (flowControlCredits > 0)
            {
                UpdateStatusLog($"CPU sends Memory Read Request TLP (Seq: {sequenceNumber})");
                await AnimatePacketAsync(440, 440, 118, 207, "Memory Read TLP", Colors.Yellow, isCPUtoGPU: true);
                await AnimatePacketAsync(440, 440, 263, 382, "Memory Read TLP", Colors.Yellow, isCPUtoGPU: true);
                flowControlCredits--;
                CreditsIndicator.Text = $"Credits: {flowControlCredits}";
                UpdateStatusLog($"GPU checks credits: {flowControlCredits} remaining");
                await AnimatePacketAsync(440, 440, 382, 263, "Completion TLP", Colors.LightGreen, isCPUtoGPU: false);
                await AnimatePacketAsync(440, 440, 207, 118, "Completion TLP", Colors.LightGreen, isCPUtoGPU: false);
                UpdateStatusLog($"GPU sends ACK DLLP for Seq: {sequenceNumber}");
                await AnimatePacketAsync(440, 440, 382, 263, "ACK DLLP", Colors.LightBlue, isCPUtoGPU: false);
                await AnimatePacketAsync(440, 440, 207, 118, "ACK DLLP", Colors.LightBlue, isCPUtoGPU: false);
                sequenceNumber++;
                flowControlCredits++;
                CreditsIndicator.Text = $"Credits: {flowControlCredits}";
                UpdateStatusLog($"Flow control credit restored: {flowControlCredits}");
            }
            else
            {
                UpdateStatusLog("Transfer blocked: No flow control credits available!");
            }
            Pkt.Visibility = Visibility.Hidden;
            PktLabel.Visibility = Visibility.Hidden;
        }
        private async void IntroduceError_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            cpuToGpuLayersHighlighted = false; // Reset for new transaction
            gpuToCpuLayersHighlighted = false; // Reset for new transaction
            if (flowControlCredits > 0)
            {
                UpdateStatusLog($"CPU sends Memory Read Request TLP (Seq: {sequenceNumber}) with CRC error");
                await AnimatePacketAsync(440, 440, 118, 207, "Memory Read TLP (Error)", Colors.Red, isCPUtoGPU: true);               
                await AnimatePacketAsync(440, 440, 263, 382, "Memory Read TLP (Error)", Colors.Red, isCPUtoGPU: true);
                flowControlCredits--;
                CreditsIndicator.Text = $"Credits: {flowControlCredits}";
                UpdateStatusLog($"GPU detects CRC error, sends NAK DLLP");
                await AnimatePacketAsync(440, 440, 382, 263, "NAK DLLP", Colors.Orange, isCPUtoGPU: false);
                await AnimatePacketAsync(440, 440, 207, 118, "NAK DLLP", Colors.Orange, isCPUtoGPU: false);
                UpdateStatusLog($"CPU retransmits TLP (Seq: {sequenceNumber})");
                cpuToGpuLayersHighlighted = false; // Reset for retransmission
                await AnimatePacketAsync(440, 440, 118, 207, "Memory Read TLP", Colors.Yellow, isCPUtoGPU: true);
                await AnimatePacketAsync(440, 440, 263, 382, "Memory Read TLP", Colors.Yellow, isCPUtoGPU: true);
                UpdateStatusLog($"GPU sends ACK DLLP for Seq: {sequenceNumber}");
                gpuToCpuLayersHighlighted = false; // Reset for new GPU-to-CPU transaction
                await AnimatePacketAsync(440, 440, 382, 263, "ACK DLLP", Colors.LightBlue, isCPUtoGPU: false);
                await AnimatePacketAsync(440, 440, 207, 118, "ACK DLLP", Colors.LightBlue, isCPUtoGPU: false);
                sequenceNumber++;
                flowControlCredits++;
                CreditsIndicator.Text = $"Credits: {flowControlCredits}";
                UpdateStatusLog($"Flow control credit restored: {flowControlCredits}");
            }
            else
            {
                UpdateStatusLog("Error simulation blocked: No flow control credits available!");
            }
            Pkt.Visibility = Visibility.Hidden;
            PktLabel.Visibility = Visibility.Hidden;

        }

        private async void MemoryWrite_Click(object sender, RoutedEventArgs e)
        {
            Log.Clear();
            cpuToGpuLayersHighlighted = false; // Reset for new transaction
            gpuToCpuLayersHighlighted = false; // Reset for new transaction
            if (flowControlCredits > 0)
            {
                UpdateStatusLog($"CPU sends Memory Write TLP (Seq: {sequenceNumber})");
                await AnimatePacketAsync(440, 440, 118, 207, "Memory Write TLP", Colors.Purple, isCPUtoGPU: true);
                await AnimatePacketAsync(440, 440, 263, 382, "Memory Write TLP", Colors.Purple, isCPUtoGPU: true);
                flowControlCredits--;
                CreditsIndicator.Text = $"Credits: {flowControlCredits}";
                UpdateStatusLog($"GPU checks credits: {flowControlCredits} remaining");
                UpdateStatusLog($"GPU sends ACK DLLP for Seq: {sequenceNumber}");
                await AnimatePacketAsync(440, 440, 382, 263, "ACK DLLP", Colors.LightBlue, isCPUtoGPU: false);
                await AnimatePacketAsync(440, 440, 207, 118, "ACK DLLP", Colors.LightBlue, isCPUtoGPU: false);
                sequenceNumber++;
                flowControlCredits++;
                CreditsIndicator.Text = $"Credits: {flowControlCredits}";
                UpdateStatusLog($"Flow control credit restored: {flowControlCredits}");
            }
            else
            {
                UpdateStatusLog("Transfer blocked: No flow control credits available!");
            }
            Pkt.Visibility = Visibility.Hidden;
            PktLabel.Visibility = Visibility.Hidden;
        }


        private async Task AnimatePacketAsync(double fx, double tx,
                                   double fy, double ty,
                                   string text, Color fill, bool isCPUtoGPU)
        {
            // Sequential layer highlighting
            var transactionLayer = isCPUtoGPU ? TransactionLayerCPU : TransactionLayerGPU;
            var dataLinkLayer = isCPUtoGPU ? DataLinkLayerCPU : DataLinkLayerGPU;
            var physicalLayer = isCPUtoGPU ? PhysicalLayerCPU : PhysicalLayerGPU;
            var device = isCPUtoGPU ? "CPU" : "GPU";

            if (isCPUtoGPU && !cpuToGpuLayersHighlighted)
            {
                UpdateStatusLog($"{device} Transaction Layer: Forming {text}");
                transactionLayer.Foreground = Brushes.Black;
                await Task.Delay(500);
                transactionLayer.Foreground = Brushes.Gray;

                UpdateStatusLog($"{device} Data Link Layer: Adding Sequence Number and CRC");
                dataLinkLayer.Foreground = Brushes.Black;
                await Task.Delay(500);
                dataLinkLayer.Foreground = Brushes.Gray;

                UpdateStatusLog($"{device} Physical Layer: Transmitting Packet");
                physicalLayer.Foreground = Brushes.Black;
                await Task.Delay(500);
                physicalLayer.Foreground = Brushes.Gray;

                cpuToGpuLayersHighlighted = true;
            }
            else if (!isCPUtoGPU && !gpuToCpuLayersHighlighted)
            {
                UpdateStatusLog($"{device} Transaction Layer: Forming {text}");
                transactionLayer.Foreground = Brushes.Black;
                await Task.Delay(500);
                transactionLayer.Foreground = Brushes.Gray;

                UpdateStatusLog($"{device} Data Link Layer: Adding Sequence Number and CRC");
                dataLinkLayer.Foreground = Brushes.Black;
                await Task.Delay(500);
                dataLinkLayer.Foreground = Brushes.Gray;

                UpdateStatusLog($"{device} Physical Layer: Transmitting Packet");
                physicalLayer.Foreground = Brushes.Black;
                await Task.Delay(500);
                physicalLayer.Foreground = Brushes.Gray;

                gpuToCpuLayersHighlighted = true;
            }

            // Packet animation
            Pkt.Fill = new SolidColorBrush(fill);
            PktLabel.Text = text;
            Pkt.Visibility = PktLabel.Visibility = Visibility.Visible;

            Canvas.SetLeft(Pkt, fx);
            Canvas.SetTop(Pkt, fy);
            Canvas.SetLeft(PktLabel, fx);
            Canvas.SetTop(PktLabel, fy - 20);

            TimeSpan dur = TimeSpan.FromSeconds(0.7);
            DoubleAnimation ax = new DoubleAnimation(fx, tx, dur);
            DoubleAnimation ay = new DoubleAnimation(fy, ty, dur);
            DoubleAnimation lx = new DoubleAnimation(fx, tx, dur);
            DoubleAnimation ly = new DoubleAnimation(fy - 20, ty - 20, dur);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            ax.Completed += (_, __) =>
            {
                Pkt.Visibility = PktLabel.Visibility = Visibility.Hidden;
                tcs.SetResult(true);
            };

            Pkt.BeginAnimation(Canvas.LeftProperty, ax);
            Pkt.BeginAnimation(Canvas.TopProperty, ay);
            PktLabel.BeginAnimation(Canvas.LeftProperty, lx);
            PktLabel.BeginAnimation(Canvas.TopProperty, ly);

            await tcs.Task;
            await Task.Delay(200);
        }

        private void UpdateStatusLog(string msg)
        {
            Log.AppendText($"{DateTime.Now:HH:mm:ss}: {msg}\n");
            Log.ScrollToEnd();
        }
    }
}