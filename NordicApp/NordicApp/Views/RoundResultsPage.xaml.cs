using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NordicApp.Data;
using NordicApp.Models;

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
            _raceGroups = createRacerGroups();
            _racers = await getRacers();
            _racers = selectRacers();
            OrganizeRacers();
            resultsViewer.ItemsSource = _raceGroups;
        }

        protected override void OnAppearing()
        {
            Title = "Results for Round " + _round.ToString();
            base.OnAppearing();
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

        private List<Racer> selectRacers()
        {
            switch (_round)
            {
                case (1):
                    var selected = from people in _racers
                                   where people.premlFinished
                                   select people;
                    return new List<Racer>(selected);
                case (2):
                    selected = from people in _racers
                               where people.roundOneFinish
                                   select people;
                    return new List<Racer>(selected);
                case (3):
                    selected = from people in _racers
                               where people.roundTwoFinish
                                   select people;
                    return new List<Racer>(selected);
                case (4):
                    selected = from people in _racers
                               where people.roundTwoFinish
                                   select people;
                    return new List<Racer>(selected);
                default:
                    return null;
            }
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

        private void OrganizeRacers()
        {


        }

        private async void nextRound_Clicked(object sender, EventArgs e)
        {
            bool done = await DisplayAlert("Check", "Finish viewing results?", "Yes", "No");
            if (done)
            {
                _round++;
                await Navigation.PushAsync(new RoundPage(_raceInfo, _round));
            }
            return;
        }
    }
}