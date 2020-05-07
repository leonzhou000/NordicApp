using System;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using NordicApp.Models;
using NordicApp.Data;


namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Race> _races;
        private Race _selectedRace;
        private int _round;

        public MainPage()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
        }

        protected override async void OnAppearing()
        {
            try
            {
                await _connection.CreateTableAsync<Race>();
                _races = await getRaces();
                raceRecord.ItemsSource = _races;
            }
            catch
            {
                await DisplayAlert("Error", "Invalid create", "OK");
            }
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async Task<ObservableCollection<Race>> getRaces()
        {
            try
            {
                var racelist = await _connection.Table<Race>().ToListAsync();
                return new ObservableCollection<Race>(racelist);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
            }
            return null;
        }

        private int getRoundNumber(Race race)
        {
            for (int i=0; i<5;i++)
            {
                if(race.getRoundStatus(i) == false)
                {
                    return i;
                }
            }
            return -1;
        }

        private bool checkRaceStatus(Race race)
        {
            for (int i = 0; i < 5; i++)
            {
                if (race.getRoundStatus(i) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private async void ContinueRace(Race race)
        {
            int round = getRoundNumber(race);
            var choose = await DisplayAlert("Alert", "Your race is incomplete.\nWould you like to continue race?", "Yes", "No");
            if (choose)
            {
                navigateToPage(round, race);
            }
            else
            {
                return;
            }
        }

        private async void navigateToPage(int round, Race race)
        {
            if(round == 0)
            {
                await Navigation.PushAsync(new PrelimaryPage(race));
            }
            else
            {
                await Navigation.PushAsync(new RoundPage(race, round));
            }
        }

        private async void viewResults()
        {
            var choose = await DisplayAlert("Alert", "Would you like to the result", "Yes", "No");
            if (choose)
            {
                await Navigation.PushAsync(new ResultsPage(_selectedRace));
            }
            else
            {
                return;
            }   
        }

        private async void raceRecord_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRace = raceRecord.SelectedItem as Race;
            if (checkRaceStatus(_selectedRace))
            {
                viewResults();
            }
            else
            {
                ContinueRace(_selectedRace);
            }
        }

        private async void addRace_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateRace());
        }

        private async void rmRace_Clicked(object sender, EventArgs e)
        {
            try
            {
                var race = raceRecord.SelectedItem as Race;
                if(race == null)
                {
                    await DisplayAlert("Alert", "No item selected", "OK");
                    return;
                }
                await _connection.DeleteAsync(race);
                _races.Remove(race);
            }
            catch
            {
                await DisplayAlert("Error","Fail to delete race.","OK");
                return;
            }
        }
    }
}