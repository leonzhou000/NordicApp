using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NordicApp.Views;

namespace NordicApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

<<<<<<< Updated upstream
            MainPage = new NavigationPage(new MainPage());
=======
            MainPage = new NavigationPage(new CreateRace());
>>>>>>> Stashed changes
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
