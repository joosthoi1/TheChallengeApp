using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.Generic;
using System.IO;

namespace App1
{
    public static class GlobalVariables
    {
        public static List<IDevice> deviceList = new List<IDevice>();
    }
    public partial class App : Application
    {
        static Database database;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "people.db3"));
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginRegister());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
