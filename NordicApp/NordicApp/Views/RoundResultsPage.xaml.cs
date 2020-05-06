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
using System.Runtime.ExceptionServices;
using System.Threading;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoundResultsPage : ContentPage
    {
        private Race _raceInfo;
        private Racer _selectedRacer;
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
            Title = getRoundString(_round) + " Results";
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
                           where people.getRoundFinish(_round - 1) && people.disqualified == false
                           orderby people.getRoundPlacement(_round) ascending
                           select people;
            foreach(var racer in selected)
            {
                racer.placement = racer.getRoundPlacement(_round)+1;
            }
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

        private void lastPlaceHandling(Racer racer)
        {
            int heat = racer.getHeatNumber(_round);
            if(heat+1 >= _totalHeatNumber)
            {
                disqualifeRacer(racer);
                return;
            }
            racer.setHeatNumber(_round + 1, heat + 1);
            racer.setRoundsLane(_round + 1, 0);
            _connection.UpdateAsync(racer);
        }

        private void firstPlaceHandling(Racer racer)
        {
            int heat = racer.getHeatNumber(_round);
            if(heat - 1 < 0)
            {
                int lane = racer.getRoundPlacement(_round);
                racer.setHeatNumber(_round + 1, heat);
                racer.setRoundsLane(_round + 1, lane);
                _connection.UpdateAsync(racer);
                return;
            }
            int last = (_raceGroups[racer.getHeatNumber(_round) - 1].Count - 1);
            racer.setHeatNumber(_round + 1, heat - 1);
            racer.setRoundsLane(_round + 1, last);
            _connection.UpdateAsync(racer);
        }

        private void setNewLanes()
        {
            foreach(var racer in _racers)
            {
                int last = (_raceGroups[racer.getHeatNumber(_round)].Count - 1);
                if (racer.getRoundPlacement(_round) == 0)
                {
                    firstPlaceHandling(racer);
                }
                else if(racer.getRoundPlacement(_round) == last)
                {
                    lastPlaceHandling(racer);
                }
                else
                {
                    int lane = racer.getRoundPlacement(_round);
                    int heat = racer.getHeatNumber(_round);
                    racer.setRoundsLane(_round + 1, lane);
                    racer.setHeatNumber(_round + 1, heat);
                    _connection.UpdateAsync(racer);
                }
            }
        }

        private void resultsViewer_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRacer = resultsViewer.SelectedItem as Racer;
            resultsViewer.SelectedItem = null;
        }

        private async void disqualifeRacer(Racer racer)
        {
            racer.disqualified = true;
            await _connection.UpdateAsync(racer);
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
            if (_selectedRacer == null)
            {
                DisplayAlert("No racer selected.", "Please choose a racer.", "OK");
                return;
            }

            Navigation.PushAsync(new ModifyPage(_raceInfo,_selectedRacer, _round));
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
                setNewLanes();
                resetStatus();
                _raceInfo.setRoundStatus(_round);
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