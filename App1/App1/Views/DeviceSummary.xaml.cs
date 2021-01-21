using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceSummary : ContentPage
    {
        public DeviceSummary()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();


            await PopulateListView();
        }
        private async Task PopulateListView()
        {
            List<Timers> timers = new List<Timers>();
            SlListView.Children.Clear();
            foreach (Schakelaar device in await App.Database.GetUserDevices(GlobalVariables.user.idRegistratie))
            {
                timers.AddRange(await App.Database.GetDeviceTimers(GlobalVariables.user.idRegistratie, device.DeviceId));
            }
            foreach (Timers timer in timers)
            {
                StackLayout sl = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center
                };
                CheckBox cb = new CheckBox()
                {
                    VerticalOptions = LayoutOptions.Center,
                    IsChecked = timer.StaatAan
                };
                Label label = new Label()
                {
                    Text = (await App.Database.GetSchakelaarByPK(timer.DeviceId)).Name.Split()[2],
                    FontSize = 30,
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromHex("#777777")
                };
                StackLayout sl1 = new StackLayout()
                {
                };
                Label label1 = new Label()
                {
                    Text = $"{timer.Stop.Hours.ToString()}:{timer.Stop.Minutes.ToString()}",
                    FontSize = 25,
                    TextColor = Color.FromHex("#f8a54a"),
                    FontAttributes = FontAttributes.Bold
                };
                Label label2 = new Label()
                {
                    Text = $"{timer.Stop.Hours.ToString()}:{timer.Stop.Minutes.ToString()}",
                    FontSize = 25,
                    TextColor = Color.FromHex("#f8a54a"),
                    FontAttributes = FontAttributes.Bold
                };
                sl1.Children.Add(label1);
                sl1.Children.Add(label2);
                sl.Children.Add(cb);
                sl.Children.Add(label);
                sl.Children.Add(sl1);
                SlListView.Children.Add(sl);
            }

            /*
            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                <CheckBox VerticalOptions="Center"/>
                <Label Text="Schakelaar 1" FontSize="30" VerticalOptions="Center" FontAttributes="Bold" TextColor="#777777"/>
                <StackLayout>
                    <Label Text="13:00" FontSize="25" TextColor="#f8a54a" FontAttributes="Bold"/>
                    <BoxView Style="{StaticResource Separator}" Margin="0,-5,0,-5" />
                    <Label Text="17:00" FontSize="25" TextColor="#f8a54a" FontAttributes="Bold"/>
                </StackLayout>
            </StackLayout>
             */
        }
        async void HomeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage());
        }
        async void TimerSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TimerSettings());
        }
        async void LogOutClicked(object sender, EventArgs e)
        {
            await ResetNavigationStack();
        }
        async Task ResetNavigationStack()
        {
            await Navigation.PushAsync(new GoodBye());
        }
    }
}