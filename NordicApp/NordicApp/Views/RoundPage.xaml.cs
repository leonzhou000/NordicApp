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
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using SQLitePCL;

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
        private int prevHeat = 0;
        private int currentHeat = 0;
        private int placement = 1;

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
            _racers = await getAllRacers();
            _racers = selectRacers();
            totalNumberOfRacers = _racers.Count;
            totalNumberOfHeats = getMaxNumberOfHeats();
            Info.Text = "Total number of Heats: "+getMaxNumberOfHeats().ToString();
            _raceGroups = createRaceGroups();
            OrganizeHeats();
            heatTracker.Text = "Heat " + (currentHeat+1).ToString() + " ready to Start.";
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

        private async Task<List<Racer>> getAllRacers()
        {
            try
            {
                var table = await _connection.Table<Racer>().ToListAsync();
                var racers = from people in table
                             where people.dataset == _raceInto.Id 
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
            
            if(_round == 1)
            {
                var selected = from people in _racers
                               where people.getRoundFinish(_round - 1) && people.disqualified == false
                               orderby people.ElapsedTime ascending
                               select people;
                return new List<Racer>(selected);
            }
            else
            {
                var selected = from people in _racers
                           where people.getRoundFinish(_round - 1) && people.disqualified == false
                           orderby people.getRoundPlacement(_round - 1) ascending
                           select people;
                return new List<Racer>(selected);
            }
        }

        private ObservableCollection<RacerGroups> createRaceGroups()
        {
            ObservableCollection<RacerGroups> _groups = new ObservableCollection<RacerGroups>();

            for (int j = 0; j < totalNumberOfHeats; j++)
            {
                string title = "Heat " + (j+1).ToString();
                _groups.Add(new RacerGroups(title, (j+1).ToString()));
            }

            return _groups;
        }

        private void OrganizeHeats()
        {
            int heat = 0;
            if (_round == 1)
            {
                for (int i = 0; i < totalNumberOfRacers; i++)
                {
                    if (_raceGroups[heat].Count == heatsize)
                    {
                        heat++;
                    }
                    _racers[i].setHeatNumber(_round, heat);
                    _raceGroups[heat].Add(_racers[i]);
                }
                return;
            }
            else
            {
                foreach (var racer in _racers)
                {
                    int prevHeatNumber = racer.getHeatNumber(_round - 1);
                    racer.setHeatNumber(_round, prevHeatNumber);
                    _raceGroups[prevHeatNumber].Add(racer);
                }
                switchRacers();
                return;
            }
        }

        private void mergeGroups()
        {
            
        }

        private void switchRacers()
        {
            int firstPlaceTemp = 0;
            int lastPlaceTemp = 5;
            Racer temp;
            for (int i = 0; i < totalNumberOfHeats; i++)
            {
                int index = (_raceGroups[i].Count - 1);
                if (i + 1 >= totalNumberOfHeats)
                {
                    disqualifeRacer(_raceGroups[i][index]);
                    return;
                }
                temp = _raceGroups[i][index];
                _raceGroups[i][index] = _raceGroups[i + 1][0];
                _raceGroups[i][index].setHeatNumber(_round, i);
                _connection.UpdateAsync(_raceGroups[i][index]);
                _raceGroups[i + 1][0] = temp;
                _raceGroups[i + 1][0].setHeatNumber(_round, i + 1);
                _connection.UpdateAsync(_raceGroups[i][0]);
            }
        }

        private void disqualifeRacer(Racer racer)
        {
            racer.disqualified = true;
            _connection.UpdateAsync(racer);
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
                _groups[_racers[i].getHeatNumber(_round)].Add(_racers[i]);
            }

            return _groups;
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
                //_raceInto.setRoundStatus(_round);
                await _connection.UpdateAsync(_raceInto);
                await Navigation.PushAsync(new RoundResultsPage(_raceInto, _round, totalNumberOfHeats));
            }
            
            return;
        }

        private async void startBtm_Clicked(object sender, EventArgs e)
        {
            if (currentHeat > totalNumberOfHeats-1)
                return;

            for(int i=0; i < _raceGroups[currentHeat].Count; i++)
            {
                _raceGroups[currentHeat][i].status = Status.Started.ToString();
            }
            
            await _connection.UpdateAllAsync(_raceGroups[currentHeat]);
            
            _raceGroups = getRaceGroups();
            ViewHeats.ItemsSource = _raceGroups;
            prevHeat = currentHeat;
            currentHeat++;

            if (currentHeat > totalNumberOfHeats - 1)
            {
                heatTracker.Text = "All Heat are done racing.\nThere are no more heats to start.";
                return;
            }
            
            heatTracker.Text = "Heat " + (currentHeat+1).ToString() + " ready to Start.";
        }

        private async void resetBtm_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("Reset heat start","Would you like to re-start the heat?","Yes","No");
            if (response)
            {
                for (int i = 0; i < _raceGroups[prevHeat].Count; i++)
                {
                    _raceGroups[prevHeat][i].status = Status.StandyBy.ToString();
                    _raceGroups[prevHeat][i].roundOneFinish = false;
                }

                await _connection.UpdateAllAsync(_raceGroups[prevHeat]);

                _raceGroups = getRaceGroups();
                ViewHeats.ItemsSource = _raceGroups;
                currentHeat = prevHeat;
                heatTracker.Text = "Heat " + (currentHeat + 1).ToString() + " ready to Start.";
            }
            
            return;
        }

        private async void stopBtm_Clicked(object sender, EventArgs e)
        {
            if (_selectedRacer == null || _selectedRacer.status == Status.StandyBy.ToString())
                return;

            if(placement > _raceGroups[prevHeat].Count)
            {
                placement = 0;
            }
            
            _selectedRacer.status = Status.Finished.ToString();
            _selectedRacer.setPlacement(_round, placement);
            _selectedRacer.setRoundFinish(_round);
            await _connection.UpdateAsync(_selectedRacer);
            
            _raceGroups = getRaceGroups();
            ViewHeats.ItemsSource = _raceGroups;
            placement++;
            _selectedRacer = null;
        }
    }
}