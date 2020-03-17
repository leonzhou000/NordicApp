using NordicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NordicApp.Data;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Races _racesinto;
        private int rounds = 1;

        public RoundPage(Races race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Races race)
        {
            _racesinto = race;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
        }

        private string getRound(int number)
        {
            return " ";
        }

        protected override void OnAppearing()
        {

            base.OnAppearing();
        }

        private void finishBtm_Clicked(object sender, EventArgs e)
        {
            rounds++;
            if (rounds == 4)
            {
                Title = "Final Round";
            }
        }
    }
}