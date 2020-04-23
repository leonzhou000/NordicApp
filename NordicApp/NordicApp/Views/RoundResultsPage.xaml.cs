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
            Title = "Results for "+getRoundString(_round);
            base.OnAppearing();
        }

        private string getRoundString(int round)
        {
            if (round == 4)
            {
                return "Final Round";
            }

            return "Round " + round.ToString();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
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
            var selected = from people in _racers
                            where people.getRoundFinish(_round-1)
                            orderby people.getRoundPlacement(_round) ascending
                            select people;
            return new List<Racer>(selected);
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
            foreach (var racer in _racers)
            {
                _raceGroups[racer.getHeatNumber(_round)].Add(racer);
            }
        }

        private void resetStatus()
        {
            string finished = Status.Finished.ToString();
            foreach(var racer in _racers)
            {
                if(racer.status == finished)
                {
                    racer.status = Status.StandyBy.ToString();
                }
                _connection.UpdateAsync(racer);
            }
        }

        private void modifyRacer_Clicked(object sender, EventArgs e)
        {

        }

        private async void exitRace_Clicked(object sender, EventArgs e)
        {
            var choose = await DisplayAlert("Exit", "Your race is not finished.\n Would like to leave?", "Yes", "No");
            if (choose)
            {
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                return;
            }
        }

        private async void nextRound_Clicked(object sender, EventArgs e)
        {
            bool done = await DisplayAlert("Check", "Finish viewing results?", "Yes", "No");
            if (done && _round < 4)
            {
                resetStatus();
                //_raceInfo.setRoundStatus(_round);
                await _connection.UpdateAsync(_raceInfo);
                _round++;
                await Navigation.PushAsync(new RoundPage(_raceInfo, _round));
            }
            else if (done && _round == 4)
            {
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            return;
        }
    }
}