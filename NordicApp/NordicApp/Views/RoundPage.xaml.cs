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
        private Race _raceInto;
        private List<Racer> _racers;
        private ObservableCollection<RacerGroups> _raceGroups;
        private int rounds = 1;
        private int totalNumberOfRacers;
        private int heatsize = 5;

        public RoundPage(Race race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Race race)
        {
            _raceInto = race;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            _racers = await getRacers();
            totalNumberOfRacers = _racers.Count;
            Info.Text = totalNumberOfRacers.ToString();
            ViewHeats.ItemsSource = getRaceGroups();
        }

        protected override void OnAppearing()
        {
            Title = getRoundString();
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
                             orderby people.ElapsedTime ascending
                             select people;
                return new List<Racer>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private ObservableCollection<RacerGroups> getRaceGroups()
        {
            ObservableCollection<RacerGroups> _temp = new ObservableCollection<RacerGroups>();
            RacerGroups heat = new RacerGroups("Heat 1", "1");
            RacerGroups heat2 = new RacerGroups("Heat 2", "2");
            RacerGroups heat3 = new RacerGroups("Heat 3", "3");

            if (totalNumberOfRacers % heatsize == 0)
            {
                _temp.Add(heat);
                _temp.Add(heat2);
                _temp.Add(heat3);
            }
            
            if (totalNumberOfRacers % heatsize == 1 || totalNumberOfRacers % heatsize == 2)
            {
                _temp.Add(heat);
                _temp.Add(heat2);
            }

            if( totalNumberOfRacers % heatsize > 2)
            {
                _temp.Add(heat);
            }
            return _temp;
        }

        private void createRaceGroup(List<Racer> RaceList, int heatNumber)
        {
            string title = "Heat "+heatNumber.ToString();
            RacerGroups group = new RacerGroups(title, heatNumber.ToString());
            
            return;
        }

        private void mergeGroups()
        {

        }

        private void switchRacers()
        {

        }

        private string getRoundString()
        {
            return "Round "+rounds.ToString();
        }
        
        private async void finishBtm_Clicked(object sender, EventArgs e)
        {
            bool done = await DisplayAlert("Check","Finish with round?", "Yes", "No");
            if (done)
            {
                if (rounds > 3)
                {
                    await Navigation.PushAsync(new ResultsPage(_raceInto));
                }
                if (rounds == 3)
                {
                    Title = "Final Round";
                    await _connection.UpdateAsync(_raceInto);
                    rounds++;
                    return;
                }
                else
                {
                    rounds++;
                    Title = getRoundString();
                    return;
                }
            }
            return;
        }
    }
}