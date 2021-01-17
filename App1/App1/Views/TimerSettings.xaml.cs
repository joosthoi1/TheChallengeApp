using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerSettings : ContentPage
    {
        public TimerSettings()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine(deviceToggle.IsToggled);
            devicePicker.ItemsSource = GlobalVariables.deviceList;
            devicePicker.SelectedIndex = 0;
            devicePicker.Title = GlobalVariables.deviceList.Count > 0 ? GlobalVariables.deviceList[0].Name ?? GlobalVariables.deviceList[0].NativeDevice.ToString() : "No devices paired";
            deviceToggle.IsEnabled = GlobalVariables.deviceList.Count > 0;

            System.Diagnostics.Debug.WriteLine(GlobalVariables.deviceList.Count);
            System.Diagnostics.Debug.WriteLine(GlobalVariables.deviceList.Count > 0);

            if (GlobalVariables.deviceList.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Groetjes van Hans AKA Captain 3e Helft");
                queryOn();
            }
            //deviceToggle.IsToggled
        }
        public async void queryOn()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("TEST0");
                IDevice device = GlobalVariables.deviceList[devicePicker.SelectedIndex];
                await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                var service = await CrossBluetoothLE.Current.Adapter.ConnectedDevices[0].GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                var characteristic0 = await service.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
                var characteristic1 = await service.GetCharacteristicAsync(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e"));
                characteristic1.ValueUpdated += async (o, args) =>
                {
                    string x = args.Characteristic.StringValue;

                    deviceToggle.IsToggled = true;

                    await characteristic1.StopUpdatesAsync();
                    await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);
                    return;
                };
                await characteristic1.StartUpdatesAsync();
                await characteristic0.WriteAsync(StringToByteArray("71756572790a"));
            }
            catch (DeviceConnectionException er)
            {
                // ... could not connect to device
            }
        }
        async void switchToggled(object sender, EventArgs e)
        {
            if (((Switch)sender).IsToggled)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("TEST0");
                    IDevice device = GlobalVariables.deviceList[devicePicker.SelectedIndex];
                    await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                    var service = await CrossBluetoothLE.Current.Adapter.ConnectedDevices[0].GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                    var characteristic0 = await service.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
                    await characteristic0.WriteAsync(StringToByteArray("6f6e0a"));
                    await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);
                }
                catch (DeviceConnectionException er)
                {
                    // ... could not connect to device
                }
            }
            else
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("TEST1");
                    IDevice device = GlobalVariables.deviceList[devicePicker.SelectedIndex];
                    await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                    var service = await CrossBluetoothLE.Current.Adapter.ConnectedDevices[0].GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                    var characteristic0 = await service.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
                    await characteristic0.WriteAsync(StringToByteArray("6f66660a"));
                    await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);

                }
                catch (DeviceConnectionException er)
                {
                    // ... could not connect to device
                }
            }
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
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
            System.Diagnostics.Debug.WriteLine("Groetjes van Hans AKA Captain 3e Helft");
        }
    }
}