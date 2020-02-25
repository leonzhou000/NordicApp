using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NordicApp.Models;
using NordicApp.Data;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private List<Races> _Races;

        public MainPage()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
        }

        private async void addRace_Clicked(object sender, EventArgs e)
        {
            try
            {
                await _connection.CreateTableAsync<Races>();
            }
            catch
            {
                await DisplayAlert("Error","Invalid create","OK");
            }
            await Navigation.PushAsync(new CreateRace());
        }

        private void rmRace_Clicked(object sender, EventArgs e)
        {

        }
    }
}