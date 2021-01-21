using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormsPinView; 
using FormsPinView.Core; 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogIn : ContentPage
    {
        private int index = 0;
        private string correctPassword = "2305";
        List<Label> labels;
        public LogIn()
        {
            InitializeComponent();
            labels = new List<Label>() { Digit0, Digit1, Digit2, Digit3 };
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            string email = "";
            if (Application.Current.Properties.ContainsKey("email"))
            {
                email = Application.Current.Properties["email"] as string;
            }
            else
            {
                Application.Current.Properties.Add("email", "");
            }
            if (Application.Current.Properties.ContainsKey("rememberEmail"))
            {
                var rememberEmail = Application.Current.Properties["rememberEmail"] as string;
                if (rememberEmail == "0")
                {
                    rememberBox.IsChecked = false;
                }
                else
                {
                    rememberBox.IsChecked = true;
                    emailEntry.Text = email;
                }
            }
            else
            {
                if (rememberBox.IsChecked)
                {
                    Application.Current.Properties.Add("rememberEmail", "0");
                }
                else
                {
                    Application.Current.Properties.Add("rememberEmail", "1");
                }
            }
        }
        private void emailEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            Application.Current.Properties["email"] = emailEntry.Text;
        }
        private void rememberBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Application.Current.Properties["rememberEmail"] = rememberBox.IsChecked ? "1" : "0";
        }
        async void OnButtonClicked(object sender, EventArgs args)
        {
            int buttonNo = Int32.Parse(((Button)sender).StyleId);
            System.Diagnostics.Debug.WriteLine(buttonNo);
            if (buttonNo == 10 && index != 0)
            {
                index -= 1;
                labels[index].Text = "0";
                labels[index].TextColor = Color.FromHex("#FAFAFA");

                
            }
            else if (buttonNo != 10)
            {
                labels[index].Text = buttonNo.ToString();
                labels[index].TextColor = Color.FromHex("#000000");
                index += 1;
            }
            if (index == 4)
            {
                System.Diagnostics.Debug.WriteLine("x");
                string password = labels[0].Text + labels[1].Text + labels[2].Text + labels[3].Text;
                bool pwcorrect = false;
                try  {
                    pwcorrect = await App.Database.ValidateLogin(emailEntry.Text, Int32.Parse(password));
                    Registratie user = await App.Database.RegisterQuerry(emailEntry.Text);
                    GlobalVariables.user = user;
                } catch
                {

                }
                
                if (pwcorrect)
                {
                    index = 0;
                    for (int i = 0; i < labels.Count; i++)
                    {
                        labels[i].Text = "0";
                        labels[i].TextColor = Color.FromHex("#FAFAFA");
                    }
                    await Navigation.PushAsync(new HomePage());
                }
                else
                {
                    index = 0;
                    await Task.Delay(200);
                    for (int i = 0; i < labels.Count; i++)
                    {
                        labels[i].Text = "0";
                        labels[i].TextColor = Color.FromHex("#FAFAFA");
                    }
                    await DisplayAlert("Error", "Email or password incorrect", "OK");
                }
            }
        }
        Boolean CheckPassWord()
        {
            
            string password = labels[0].Text + labels[1].Text + labels[2].Text + labels[3].Text;
            if (password == correctPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
    }
}