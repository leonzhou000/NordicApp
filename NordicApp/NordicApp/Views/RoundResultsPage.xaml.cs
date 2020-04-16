using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NordicApp.Data;
using NordicApp.Models;
using System.Collections.ObjectModel;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundResultsPage : ContentPage
    {
        private Race _raceInfo;
        private SQLiteAsyncConnection _connection;
        private List<Racer> _racers;
        private ObservableCollection<RacerGroups> _raceGroups;
        private int _round;
        private int _totalHeatNumber;

        public RoundResultsPage(Race race, int round, int totalNumberOfHeats)
        {
            InitializeComponent();
            Init(race, round, totalNumberOfHeats);
        }

        private async void Init(Race race, int Round, int totalHeats)
        {
            _raceInfo = race;
            _round = Round;
            _totalHeatNumber = totalHeats;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            _racers = await getRacers();
            _raceGroups = createRacerGroups();
            resultsViewer.ItemsSource = _raceGroups;
        }

        protected override void OnAppearing()
        {
            Title = "Results for Round " + _round.ToString();
            base.OnAppearing();
        }

        private void RoundUpdater(int round)
        {
            if(round == 1)
            {
                return;
            }
            
            if(round == 2)
            {
                return;
            }
            
            if(round == 3)
            {
                return;
            }

            if(round == 4)
            {
                return;
            } 
        }

        private async Task<List<Racer>> getRacers()
        {
            try
            {
                var table = await _connection.Table<Racer>().ToListAsync();
                var racers = from people in table
                             where people.dataset == _raceInfo.Id 
                             select people;
                return new List<Racer>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private void organizeRacers(ObservableCollection<RacerGroups> group, int heat)
        {

        }

        private ObservableCollection<RacerGroups> createRacerGroups()
        {
            ObservableCollection<RacerGroups> _groups = new ObservableCollection<RacerGroups>();
            for (int j = 0; j < _totalHeatNumber; j++)
            {
                string title = "Heat " + (j + 1).ToString();
                _groups.Add(new RacerGroups(title, (j + 1).ToString()));
            }

            return _groups; 
        }

        private async void nextRound_Clicked(object sender, EventArgs e)
        {
            bool done = await DisplayAlert("Check", "Finish viewing results?", "Yes", "No");
            if (done)
            {
                await Navigation.PushAsync(new RoundPage(_raceInfo, 2));
            }
            return;
        }
    }
}