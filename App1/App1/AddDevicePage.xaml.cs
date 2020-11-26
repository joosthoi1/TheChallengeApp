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
    public partial class AddDevicePage : ContentPage
    {
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
    }
}