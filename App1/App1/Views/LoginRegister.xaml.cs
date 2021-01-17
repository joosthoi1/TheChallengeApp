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
    public partial class LoginRegister : ContentPage
    {
        public LoginRegister()
        {
            InitializeComponent();
        }

        private async void RegistreerClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LogIn());
        }
    }
}