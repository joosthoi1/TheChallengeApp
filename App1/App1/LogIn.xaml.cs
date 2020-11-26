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
                if (CheckPassWord())
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