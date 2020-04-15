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
    public partial class RoundPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Race _raceInto;
        private Racer _selectedRacer;
        private List<Racer> _racers;
        private ObservableCollection<RacerGroups> _raceGroups;
        private int _round;
        private int totalNumberOfRacers;
        private int totalNumberOfHeats;
        private int heatsize = 5;
        private int prevRound = 0;
        private int currentRound = 0;
        private int placemant = 1;

        public RoundPage(Race race, int Round)
        {
            InitializeComponent();
            Init(race, Round);
        }

        private async void Init(Race race, int Round)
        {
            _raceInto = race;
            _round = Round;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            _racers = await getRacers();
            totalNumberOfRacers = _racers.Count;
            Info.Text = "Total number of Heats: "+getMaxNumberOfHeats().ToString();
            heatTracker.Text = "Heat " + (currentRound+1).ToString() + " ready to Start.";
            totalNumberOfHeats = getMaxNumberOfHeats();
            _raceGroups = createRaceGroups();
            ViewHeats.ItemsSource = _raceGroups;
        }
        protected override void OnAppearing()
        {
            Title = getRoundString(_round);
            base.OnAppearing();
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
                             where people.dataset == _raceInto.Id && people.disqualified == false
                             orderby people.ElapsedTime descending
                             select people;
                return new List<Racer>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }
        private ObservableCollection<RacerGroups> createRaceGroups()
        {
            ObservableCollection<RacerGroups> _groups = new ObservableCollection<RacerGroups>();
            int heat = 0;

            for (int j = 0; j < totalNumberOfHeats; j++)
            {
                string title = "Heat " + (j+1).ToString();
                _groups.Add(new RacerGroups(title, (j+1).ToString()));
            }

            for (int i = 0; i < totalNumberOfRacers; i++)
            {
                if (_groups[heat].Count == heatsize)
                {
                    heat++;
                }
                _racers[i].setRecordHeat(_round, heat);
                _groups[heat].Add(_racers[i]);
            }
            _connection.UpdateAllAsync(_groups);
            return _groups;
        }

        private ObservableCollection<RacerGroups> getRaceGroups()
        {
            ObservableCollection<RacerGroups> _groups = new ObservableCollection<RacerGroups>();
            
            for (int j = 0; j < totalNumberOfHeats; j++)
            {
                string title = "Heat " + (j + 1).ToString();
                _groups.Add(new RacerGroups(title, (j + 1).ToString()));
            }

            for (int i = 0; i < totalNumberOfRacers; i++)
            {
                _groups[_racers[i].getRoundHeatNumber(_round)].Add(_racers[i]);
            }

            return _groups;
        }

        private void mergeGroups()
        {

        }

        private void switchRacers()
        {

        }

        private int getMaxNumberOfHeats()
        {
            if (totalNumberOfRacers % 5 == 0)
            {
                return totalNumberOfRacers / 5;
            }
            else
            {
                return (totalNumberOfRacers / 5) + 1;
            }
        }

        private string getRoundString(int round)
        {
            if(round == 4)
            {
                return "Final Round";
            }
            
            return "Round "+round.ToString();
        }

        private void ViewHeats_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRacer = ViewHeats.SelectedItem as Racer;
            ViewHeats.SelectedItem = null;
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

        private async void finishBtm_Clicked(object sender, EventArgs e)
        {
            bool done = await DisplayAlert("Check","Finish with round?", "Yes", "No");
            if (done)
            {
                await Navigation.PushAsync(new RoundResultsPage(_raceInto, _round));
            }
            
            return;
        }

        private async void startBtm_Clicked(object sender, EventArgs e)
        {
            if (currentRound > totalNumberOfHeats-1)
                return;

            for(int i=0; i < _raceGroups[currentRound].Count; i++)
            {
                _raceGroups[currentRound][i].status = "started";
            }
            
            await _connection.UpdateAllAsync(_raceGroups[currentRound]);
            _racers = await getRacers();
            _raceGroups = getRaceGroups();
            ViewHeats.ItemsSource = _raceGroups;
            prevRound = currentRound;
            currentRound++;

            if (currentRound > totalNumberOfHeats - 1)
            {
                heatTracker.Text = "All Heat are done racing.\nThere are no more heats to start.";
                return;
            }
            
            heatTracker.Text = "Heat " + (currentRound+1).ToString() + " ready to Start.";
        }

        private async void resetBtm_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("Reset heat start","Would you like to re-start the heat?","Yes","No");
            if (response)
            {
                for (int i = 0; i < _raceGroups[prevRound].Count; i++)
                {
                    _raceGroups[prevRound][i].status = "Standby";
                }
                await _connection.UpdateAllAsync(_raceGroups[prevRound]);
                _racers = await getRacers();
                _raceGroups = getRaceGroups();
                ViewHeats.ItemsSource = _raceGroups;
                currentRound = prevRound;
                heatTracker.Text = "Heat " + (currentRound + 1).ToString() + " ready to Start.";
            }
            
            return;
        }

        private async void stopBtm_Clicked(object sender, EventArgs e)
        {
            if (_selectedRacer == null)
                return;

            if(placemant > 5)
            {
                placemant = 0;
            }
            
            _selectedRacer.status = "Finished";
            _selectedRacer.roundOneFinish = placemant;
            await _connection.UpdateAsync(_selectedRacer);
            _racers = await getRacers();
            _raceGroups = getRaceGroups();
            ViewHeats.ItemsSource = _raceGroups;
            placemant++;
            _selectedRacer = null;
        }
    }
}