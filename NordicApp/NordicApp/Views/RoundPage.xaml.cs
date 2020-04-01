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
using System.Collections.ObjectModel;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Races _raceInto;
        private int rounds = 1;

        public RoundPage(Races race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Races race)
        {
            _raceInto = race;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
        }

        private async Task<ObservableCollection<Racers>> getRaces()
        {
            try
            {
                var table = await _connection.Table<Racers>().ToListAsync();
                var racers = from people in table
                             where people.dataset == _raceInto.Id && people.disqualified == false
                             orderby people.Number ascending
                             select people;
                return new ObservableCollection<Racers>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private string getRoundString()
        {
            return rounds.ToString();
        }

        protected override void OnAppearing()
        {
            Title = getRoundString();
            base.OnAppearing();
        }

        private void UpdateInfo()
        {

        }

        private async void finishBtm_Clicked(object sender, EventArgs e)
        {
            bool done = await DisplayAlert("Check","Finish with round?", "Yes", "No");
            if (done)
            {
                if (rounds == 4)
                {
                    Title = "Final Round";
                    await _connection.UpdateAsync(_raceInto);
                }
                if (rounds > 4)
                {

                    await Navigation.PushAsync(new ResultsPage(_raceInto));
                }
                rounds++;
            }
            return;
        }
    }
}