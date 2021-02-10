using NordicApp.Data;
using NordicApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrelimResultsPage : ContentPage
    {
        private Racer _selectedRacer;
        private SQLiteAsyncConnection _connection;
        private Race _raceInfo;
        private ObservableCollection<Racer> _racers;
        private int _round;
        private int rank;

        public PrelimResultsPage(Race race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Race race)
        {
            _raceInfo = race;
            _round = 0;
            Title = "Prelimary Round Results";
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            _racers = await getRacers();
            rank = _racers.Count;
            resultsViewer.ItemsSource = _racers;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async Task<ObservableCollection<Racer>> getRacers()
        {
            try
            {
                var table = await _connection.Table<Racer>().ToListAsync();
                var racers = from people in table
                               where people.getRoundFinish(_round) && people.dataset == _raceInfo.Id 
                               orderby people.ElapsedTime ascending
                               select people;
                return new ObservableCollection<Racer>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private void resultsViewer_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRacer = resultsViewer.SelectedItem as Racer;
            resultsViewer.SelectedItem = null;
        }

        private async void ContinueBtm_Clicked(object sender, EventArgs e)
        {
            var choose = await DisplayAlert("Continue", "Are done view revising the results?", "Yes", "No");
            if (choose)
            {
                _raceInfo.setRoundStatus(_round);
                _round++;
                await _connection.UpdateAsync(_raceInfo);
                await Navigation.PushAsync(new RoundPage(_raceInfo, _round));
            }
            else
            {
                return;
            }
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

        private void modifyRacer_Clicked(object sender, EventArgs e)
        {
            if(_selectedRacer == null)
            {
                DisplayAlert("No racer selected.","Please choose a racer.", "OK");
                return;
            }

            Navigation.PushAsync(new ModifyPage(_raceInfo, _selectedRacer, _round));
        }
    }
}