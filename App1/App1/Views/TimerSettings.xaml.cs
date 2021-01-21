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
using System.Threading;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimerSettings : ContentPage
    {
        public TimerSettings()
        {
            InitializeComponent();
            
            //deviceToggle.IsToggled
        }
        private static List<Schakelaar> currentSchakelaars;
        protected override void OnAppearing()
        {
            base.OnAppearing();

            System.Diagnostics.Debug.WriteLine(deviceToggle.IsToggled);
            currentSchakelaars = App.Database.GetUserDevices(GlobalVariables.user.idRegistratie).GetAwaiter().GetResult();
            List<String> list = currentSchakelaars.Select(item => item.Name.Split(' ')[2]).ToList();
            devicePicker.ItemsSource = list;
            devicePicker.SelectedIndex = 0;

            devicePicker.Title = currentSchakelaars.Count > 0 ? currentSchakelaars[0].Name ?? currentSchakelaars[0].DeviceGUID : "No devices paired";
            deviceToggle.IsEnabled = currentSchakelaars.Count > 0;

            System.Diagnostics.Debug.WriteLine(currentSchakelaars.Count);
            System.Diagnostics.Debug.WriteLine(currentSchakelaars.Count > 0);
            /*
            if (currentSchakelaars.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Groetjes van Hans AKA Captain 3e Helft");
                queryOn();
            }
            */
            PopulateListView();
        }
        private async Task PopulateListView()
        {
            timerScrollView.Children.Clear();
            List<Timers> timers = await App.Database.GetDeviceTimers(GlobalVariables.user.idRegistratie, currentSchakelaars[devicePicker.SelectedIndex].DeviceId);
            for (int i =0; i < timers.Count; i++)
            {
                Frame frame = new Frame()
                {
                    BorderColor = Color.FromHex("#777777"),
                    BackgroundColor = Color.FromHex("#FAFAFA"),
                };
                StackLayout stackLayout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                TimePicker tpStart = new TimePicker()
                {
                    Time = timers[i].Start,
                    TextColor = Color.FromHex("#f8a54a"),
                    FontSize = 25,
                    FontAttributes = FontAttributes.Bold
                };
                Label minusLabel = new Label()
                {
                    Text = $"-",
                    TextColor = Color.FromHex("#f8a54a"),
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 25,
                };
                TimePicker tpStop = new TimePicker()
                {
                    Time = timers[i].Stop,
                    TextColor = Color.FromHex("#f8a54a"),
                    FontSize = 25,
                    FontAttributes = FontAttributes.Bold
                };
                Switch s = new Switch()
                {
                    IsToggled = timers[i].StaatAan
                };
                var saveButton = new ImageButton()
                {
                    Source = ImageSource.FromFile("SaveButton.png"),
                    BackgroundColor = Color.FromHex("#FAFAFA"),
                    HeightRequest = 30
                };
                saveButton.Clicked += SaveButtonClicked;
                s.Toggled += SwitchClicked;
                frame.Content = stackLayout;
                stackLayout.StyleId = timers[i].TimerId.ToString();
                stackLayout.Children.Add(tpStart);
                stackLayout.Children.Add(minusLabel);
                stackLayout.Children.Add(tpStop);
                stackLayout.Children.Add(s);
                stackLayout.Children.Add(saveButton);
                timerScrollView.Children.Add(frame);
            }
            /*
            <Frame BorderColor="#777777" BackgroundColor="#FAFAFA">
                <StackLayout Orientation="Horizontal" >
                    <TimePicker TextColor="#f8a54a" FontSize="30" FontAttributes="Bold"/>
                    <Label Text="-" TextColor="#f8a54a" FontAttributes="Bold" FontSize="30"/>
                    <TimePicker TextColor="#f8a54a" FontSize="30" FontAttributes="Bold"/>
                    <Switch/>
                </StackLayout>
            </Frame>
            */
        }
        async void SwitchClicked(object s, EventArgs e)
        {

            Switch sw = (Switch)s;
            StackLayout sl = ((StackLayout)sw.Parent);
            int pk = Int32.Parse(sl.StyleId);
            Timers curTimer = await App.Database.GetTimerByPK(pk);
            Timers timer = new Timers
            {
                TimerId = pk,
                Start = ((TimePicker)sl.Children[0]).Time,
                Stop = ((TimePicker)sl.Children[2]).Time,
                DeviceId = curTimer.DeviceId,
                UserId = curTimer.UserId,
                StaatAan = sw.IsToggled
            };
            await App.Database.UpdateTimer(timer);

            Timers currentTimer = await App.Database.GetTimerByPK(pk);
            System.Diagnostics.Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(currentTimer));
            System.Diagnostics.Debug.WriteLine(DateTime.Now.TimeOfDay.TotalMinutes);
            if (sw.IsToggled)
            {
                IDevice device = await CrossBluetoothLE.Current.Adapter.ConnectToKnownDeviceAsync(Guid.Parse(currentSchakelaars[devicePicker.SelectedIndex].DeviceGUID));
                await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                var service = await device.GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                var characteristic0 = await service.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
                await characteristic0.WriteAsync(Encoding.ASCII.GetBytes("timer " + Math.Floor(DateTime.Now.TimeOfDay.TotalMinutes).ToString() + " " + Math.Floor(timer.Start.TotalMinutes).ToString() + " " + Math.Floor(timer.Stop.TotalMinutes).ToString()).Concat(StringToByteArray("0a")).ToArray());
                await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);
            }
        }
        async void SaveButtonClicked(object s, EventArgs e)
        {
            ImageButton imgB = (ImageButton)s;
            System.Diagnostics.Debug.WriteLine(((StackLayout)imgB.Parent).StyleId);
            StackLayout sl = ((StackLayout)imgB.Parent);
            int pk = Int32.Parse(sl.StyleId);
            Timers curTimer = await App.Database.GetTimerByPK(pk);
            Timers timer = new Timers
            {
                TimerId = pk,
                Start = ((TimePicker)sl.Children[0]).Time,
                Stop = ((TimePicker)sl.Children[2]).Time,
                DeviceId = curTimer.DeviceId,
                UserId = curTimer.UserId,
                StaatAan = curTimer.StaatAan
            };
            System.Diagnostics.Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(timer));
            
            await App.Database.UpdateTimer(timer);
        }
        public async void IndexChanged(object s, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("index changed");
            if (currentSchakelaars.Count > 0)
            {
                queryOn();
            }
        }
        public async Task queryOn()
        {
            await PopulateListView();
            deviceToggle.IsEnabled = false;
            try
            {
                System.Diagnostics.Debug.WriteLine("QUERYING");
                System.Diagnostics.Debug.WriteLine(devicePicker.SelectedIndex);
                System.Diagnostics.Debug.WriteLine(currentSchakelaars.Count);
                IDevice device = await CrossBluetoothLE.Current.Adapter.ConnectToKnownDeviceAsync(Guid.Parse(currentSchakelaars[devicePicker.SelectedIndex].DeviceGUID));
                await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                var service = await CrossBluetoothLE.Current.Adapter.ConnectedDevices[0].GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
                var characteristic0 = await service.GetCharacteristicAsync(Guid.Parse("6e400003-b5a3-f393-e0a9-e50e24dcca9e"));
                var characteristic1 = await service.GetCharacteristicAsync(Guid.Parse("6e400002-b5a3-f393-e0a9-e50e24dcca9e"));
                characteristic1.ValueUpdated += async (o, args) =>
                {
                    Thread.Sleep(100);
                    Device.BeginInvokeOnMainThread(() => characteristicValueChange(args, characteristic1, device));
                };
                await characteristic1.StartUpdatesAsync();
                await characteristic0.WriteAsync(StringToByteArray("71756572790a"));
            }
            catch (DeviceConnectionException er)
            {
                // ... could not connect to device
            }
            deviceToggle.IsEnabled = true;
        }
        private async void characteristicValueChange(Plugin.BLE.Abstractions.EventArgs.CharacteristicUpdatedEventArgs args, ICharacteristic characteristic1, IDevice device)
        {
            
            string x = args.Characteristic.StringValue;
            System.Diagnostics.Debug.WriteLine(x);
            await characteristic1.StopUpdatesAsync();
            await CrossBluetoothLE.Current.Adapter.DisconnectDeviceAsync(device);

            deviceToggle.IsToggled = x[0] == '1';
            return;
            
        }
        async void switchToggled(object sender, EventArgs e)
        {
            if (((Switch)sender).IsToggled)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("TEST0");
                    IDevice device = await CrossBluetoothLE.Current.Adapter.ConnectToKnownDeviceAsync(Guid.Parse(currentSchakelaars[devicePicker.SelectedIndex].DeviceGUID));
                    await CrossBluetoothLE.Current.Adapter.ConnectToDeviceAsync(device);
                    var service = await device.GetServiceAsync(Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"));
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
                    IDevice device = await CrossBluetoothLE.Current.Adapter.ConnectToKnownDeviceAsync(Guid.Parse(currentSchakelaars[devicePicker.SelectedIndex].DeviceGUID));
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
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                System.Diagnostics.Debug.WriteLine(Convert.ToByte(hex.Substring(i, 2), 16));
            }

            return bytes;
        }
        private async void AddTimer(object sender, EventArgs e)
        {
            Timers timer = new Timers
            {
                Start = DateTime.Now.TimeOfDay,
                Stop = DateTime.Now.TimeOfDay,
                StaatAan = false,
                DeviceId = currentSchakelaars[devicePicker.SelectedIndex].DeviceId,
                UserId = GlobalVariables.user.idRegistratie
            };
            await App.Database.AddTimer(timer);
            await PopulateListView();
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