using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDevicePage : ContentPage
    {
        List<IDevice> deviceList = new List<IDevice>();
        public AddDevicePage()
        {
            InitializeComponent();
        }
        async void HomeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage());
        }
        async void LogOutClicked(object sender, EventArgs e)
        {
            await ResetNavigationStack();
        }
        async Task ResetNavigationStack()
        {
            await Navigation.PushAsync(new GoodBye());

        }
        async void ScanDevices(object sender, EventArgs e)
        {
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            System.Diagnostics.Debug.WriteLine("0");
            deviceList.Clear();
            DevicesStack.Children.Clear();

            adapter.ScanMode = ScanMode.Balanced;
            
            adapter.DeviceDiscovered += discoveredDevice;
            await adapter.StartScanningForDevicesAsync();

            foreach (IDevice device in deviceList)
            {
                System.Diagnostics.Debug.WriteLine("-");
                System.Diagnostics.Debug.WriteLine(device.Id);
                System.Diagnostics.Debug.WriteLine(device.Name);
                System.Diagnostics.Debug.WriteLine(device.NativeDevice);
                System.Diagnostics.Debug.WriteLine(device.Rssi);
                System.Diagnostics.Debug.WriteLine(device.State);
                System.Diagnostics.Debug.WriteLine(device);
                System.Diagnostics.Debug.WriteLine("-");
            }
            System.Diagnostics.Debug.WriteLine("1");
            System.Diagnostics.Debug.WriteLine(deviceList.Count);
            

        }
        async void discoveredDevice(object s, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs a)
        {
            //BorderColor="#777777" BorderRadius="10" BorderWidth="2" BackgroundColor="#FAFAFA"
            deviceList.Add(a.Device);
            Button button = new Button()
            {
                Text = a.Device.Name ?? a.Device.NativeDevice.ToString(),
                FontSize = 30,
                Padding=new Thickness(0,10),
                Margin=new Thickness(10,0),
                TextTransform= TextTransform.None,
                BorderColor=Color.FromHex("#777777"),
                BorderWidth=2,
                BackgroundColor= Color.FromHex("#FAFAFA"),
                CornerRadius=2,
                TextColor=Color.FromHex("#f8a54a")
            };
            button.Clicked += (z, x) => DeviceButtonClicked(z, x, a.Device);
            DevicesStack.Children.Add(button);
        }
        async void DeviceButtonClicked(object sender, EventArgs a, IDevice device)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(device.Id);
                await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);
                GlobalVariables.deviceList.Add(device);
            }
            catch (DeviceConnectionException e)
            {
                // ... could not connect to device
            }
            /*
            var service = await CrossBluetoothLE.Current.Adapter.ConnectedDevices[0].GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
            var characteristic0 = await service.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
            var characteristic1 = await service.GetCharacteristicAsync(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e"));
            await characteristic0.WriteAsync(StringToByteArray("034a6a0a0d"));
            characteristic1.ValueUpdated += (o, args) =>
            {
                System.Diagnostics.Debug.WriteLine(args.Characteristic.StringValue);
            };
            await characteristic1.StartUpdatesAsync();
            */
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}