using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Diploma
{
    public partial class DMISimulationPage : UserControl
    {
        private int sequenceNumber = 1;
        private bool isLinkInitialized = false;
        private bool cpuToChipsetLayersHighlighted = false;
        private bool chipsetToCpuLayersHighlighted = false;

        public DMISimulationPage()
        {
            InitializeComponent();
            BandwidthIndicator.Text = "Bandwidth: 3.94 GB/s (Limited by DMI)";
            UpdateStatusLog("Initialized DMI simulation with 3.94 GB/s bandwidth limit.");
        }

        private void InitializeLink_Click(object sender, RoutedEventArgs e)
        {
            if (!isLinkInitialized)
            {
                StatusLog.Clear();
                UpdateStatusLog("Starting DMI link training (limited to 3.94 GB/s bandwidth)...");
                UpdateStatusLog("Physical Layer: Detecting link partner (CPU ↔ Chipset)");
                Task.Delay(500).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() => UpdateStatusLog("Physical Layer: Negotiating lane width (x4)"));
                });
                Task.Delay(1000).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() => UpdateStatusLog("Physical Layer: Setting link speed (8 GT/s)"));
                });
                Task.Delay(1500).ContinueWith(_ =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        UpdateStatusLog("Link training complete. Link is up with 3.94 GB/s limit!");
                        isLinkInitialized = true;
                        StartTransferButton.IsEnabled = true;
                        BottleneckButton.IsEnabled = true;
                        MemoryWriteButton.IsEnabled = true;
                    });
                });
            }
            else
            {
                UpdateStatusLog("Link already initialized with 3.94 GB/s limit!");
            }
        }

        private async Task AnimatePacketAsync(double fromX, double toX, double fromY, double toY, string packetType, string fillColor, bool isCPUtoChipset, int extraDelay = 0)
        {
            var transactionLayer = isCPUtoChipset ? TransactionLayerCPU : TransactionLayerChipset;
            var dataLinkLayer = isCPUtoChipset ? DataLinkLayerCPU : DataLinkLayerChipset;
            var physicalLayer = isCPUtoChipset ? PhysicalLayerCPU : PhysicalLayerChipset;
            var packet = isCPUtoChipset ? Packet : SsdPacket;
            var packetLabel = isCPUtoChipset ? PacketLabel : SsdPacketLabel;

            if (isCPUtoChipset && !cpuToChipsetLayersHighlighted)
            {
                UpdateStatusLog($"{(isCPUtoChipset ? "CPU" : "Chipset")} Transaction Layer: Forming {packetType}");
                transactionLayer.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(500);
                transactionLayer.Foreground = new SolidColorBrush(Colors.Gray);

                UpdateStatusLog($"{(isCPUtoChipset ? "CPU" : "Chipset")} Data Link Layer: Adding Sequence Number");
                dataLinkLayer.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(500);
                dataLinkLayer.Foreground = new SolidColorBrush(Colors.Gray);

                UpdateStatusLog($"{(isCPUtoChipset ? "CPU" : "Chipset")} Physical Layer: Transmitting Packet");
                physicalLayer.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(500);
                physicalLayer.Foreground = new SolidColorBrush(Colors.Gray);

                cpuToChipsetLayersHighlighted = true;
            }
            else if (!isCPUtoChipset && !chipsetToCpuLayersHighlighted)
            {
                UpdateStatusLog($"{(isCPUtoChipset ? "CPU" : "Chipset")} Transaction Layer: Forming {packetType} Response");
                transactionLayer.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(500);
                transactionLayer.Foreground = new SolidColorBrush(Colors.Gray);

                UpdateStatusLog($"{(isCPUtoChipset ? "CPU" : "Chipset")} Data Link Layer: Adding Sequence Number");
                dataLinkLayer.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(500);
                dataLinkLayer.Foreground = new SolidColorBrush(Colors.Gray);

                UpdateStatusLog($"{(isCPUtoChipset ? "CPU" : "Chipset")} Physical Layer: Transmitting Packet");
                physicalLayer.Foreground = new SolidColorBrush(Colors.Black);
                await Task.Delay(500);
                physicalLayer.Foreground = new SolidColorBrush(Colors.Gray);

                chipsetToCpuLayersHighlighted = true;
            }

            packet.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColor));
            packetLabel.Text = packetType;
            packet.Visibility = Visibility.Visible;
            packetLabel.Visibility = Visibility.Visible;

            Canvas.SetLeft(packet, fromX);
            Canvas.SetTop(packet, fromY);
            Canvas.SetLeft(packetLabel, fromX);
            Canvas.SetTop(packetLabel, fromY - 20);

            var duration = TimeSpan.FromSeconds(isCPUtoChipset && extraDelay > 0 ? 2 : 1); // Slower during bottleneck
            var xAnimation = new DoubleAnimation { From = fromX, To = toX, Duration = new Duration(duration), AutoReverse = false };
            var yAnimation = new DoubleAnimation { From = fromY, To = toY, Duration = new Duration(duration), AutoReverse = false };
            var labelXAnimation = new DoubleAnimation { From = fromX, To = toX, Duration = new Duration(duration), AutoReverse = false };
            var labelYAnimation = new DoubleAnimation { From = fromY - 20, To = toY - 20, Duration = new Duration(duration), AutoReverse = false };

            var tcs = new TaskCompletionSource<bool>();
            xAnimation.Completed += (s, e) =>
            {
                packet.Visibility = Visibility.Hidden;
                packetLabel.Visibility = Visibility.Hidden;
                tcs.SetResult(true);
            };

            packet.BeginAnimation(Canvas.LeftProperty, xAnimation);
            packet.BeginAnimation(Canvas.TopProperty, yAnimation);
            packetLabel.BeginAnimation(Canvas.LeftProperty, labelXAnimation);
            packetLabel.BeginAnimation(Canvas.TopProperty, labelYAnimation);

            await tcs.Task;
            await Task.Delay(500 + extraDelay);
        }

        private async void StartTransfer_Click(object sender, RoutedEventArgs e)
        {
            StatusLog.Clear();
            cpuToChipsetLayersHighlighted = false;
            chipsetToCpuLayersHighlighted = false;
            UpdateStatusLog($"CPU sends Memory Read Request to DDR4 Memory (Seq: {sequenceNumber})");
            await AnimatePacketAsync(415, 415, 160, 250, "Memory Read", "Yellow", true);
            UpdateStatusLog($"Chipset processes request, offloading to SSD for feature extraction");
            await AnimatePacketAsync(560, 500, 280, 280, "Feature Data", "Green", true); // SSD to Chipset
            UpdateStatusLog($"Chipset forwards reduced data to CPU");
            await AnimatePacketAsync(415, 415, 250, 160, "Memory Read", "Yellow", false);
            sequenceNumber++;
            UpdateStatusLog($"Transfer complete with SSD offload: Seq {sequenceNumber - 1}");
        }

        private async void Bottleneck_Click(object sender, RoutedEventArgs e)
        {
            StatusLog.Clear();
            cpuToChipsetLayersHighlighted = false;
            chipsetToCpuLayersHighlighted = false;
            UpdateStatusLog($"CPU sends large Memory Read Request to DDR4 Memory (Seq: {sequenceNumber})");
            await AnimatePacketAsync(415, 415, 160, 250, "Bottleneck", "Red", true, 2000);
            UpdateStatusLog($"Chipset delays due to 3.94 GB/s bandwidth limit");
            await AnimatePacketAsync(415, 415, 250, 160, "Bottleneck", "Red", false, 2000);
            sequenceNumber++;
            UpdateStatusLog($"Bottleneck transfer complete: Seq {sequenceNumber - 1}");
        }

        private async void MemoryWrite_Click(object sender, RoutedEventArgs e)
        {
            StatusLog.Clear();
            cpuToChipsetLayersHighlighted = false;
            chipsetToCpuLayersHighlighted = false;
            UpdateStatusLog($"CPU sends Memory Write Request to DDR4 Memory (Seq: {sequenceNumber})");
            await AnimatePacketAsync(415, 415, 160, 250, "Memory Write", "Purple", true);
            UpdateStatusLog($"Chipset acknowledges write");
            await AnimatePacketAsync(415, 415, 250, 160, "Memory Write", "Purple", false);
            sequenceNumber++;
            UpdateStatusLog($"Write complete: Seq {sequenceNumber - 1}");
        }

        private void UpdateStatusLog(string message)
        {
            StatusLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
            StatusLog.ScrollToEnd();
        }
    }
}