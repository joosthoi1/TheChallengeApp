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
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        async void AddDeviceClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddDevicePage());
        }
        async void DeviceSummaryClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeviceSummary());
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