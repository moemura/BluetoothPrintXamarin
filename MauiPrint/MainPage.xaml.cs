#if ANDROID
using Android.Bluetooth;
using Android.Content;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using Java.Lang;
using Java.Util;
#endif

namespace MauiPrint
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
#if ANDROID
            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter != null && bluetoothAdapter.IsEnabled)
            {
                var devices = bluetoothAdapter.BondedDevices;
                foreach (var device in devices)
                {
                    if (device.Name == "InnerPrinter")
                    {
                        var innerPrinterDevice = device;
                        // Connect to the device
                        var socket = innerPrinterDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"));
                        socket?.Connect();
                        // Use the socket to send data

                        var ep = new EPSON();
                        var buffer = ByteSplicer.Combine(
                                    ep.PrintLine("Receipt"),
                                    ep.Print("From "), ep.SetStyles(PrintStyle.Bold), ep.Print($"Division)"), ep.SetStyles(PrintStyle.None),
                                    ep.SetBarcodeHeightInDots(48), ep.PrintBarcode(BarcodeType.CODE39, "TES123"), ep.PrintLine(""),
                                    ep.LeftAlign(), ep.Print("item"), ep.PrintLine(""), ep.PrintLine(""), ep.PrintLine("")
                                    );
                        await socket?.OutputStream?.WriteAsync(buffer, 0, buffer.Length);
                        socket.Close();
                    }
                }
            }
#endif
        }
    }

}
