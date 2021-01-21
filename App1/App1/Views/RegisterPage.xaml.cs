using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void Registratie_Clicked(object sender, EventArgs e)
        {
            List<string> test = new List<string>()
            {
                voorNaam.Text,
                achterNaam.Text,
                postcode.Text,
                straat.Text,
                straatNo.Text,
                email.Text,
                code0.Text,
                code1.Text,
                wPlaats.Text
            };
            DateTime x = geboorteDatum.Date;
            if (code0.Text != code1.Text)
            {
                await DisplayAlert("Error", "passwords are not the same", "OK");
                return;
            }
            Registratie r = new Registratie() {
                Achternaam = achterNaam.Text,
                Voornaam = voorNaam.Text,
                Emailadres = email.Text.ToLower(),
                Gebruikerscode = Int32.Parse(code0.Text),
                Geboortedatum = geboorteDatum.Date,
                Postcode = postcode.Text,
                Woonplaats = wPlaats.Text,
            };
            await App.Database.RegisterPerson(r);

            Application.Current.Properties["email"] = email.Text.ToLower();
            await Navigation.PushAsync(new LogIn());
        }
    }
}
