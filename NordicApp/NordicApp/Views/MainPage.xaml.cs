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

        private async void CheckRaceStatus(Race race)
        {
            if (race.Prelimary == false)
            {
                await Navigation.PushAsync(new PrelimaryPage(race));
            }
            else if( race.roundOne == false)
            {
                await Navigation.PushAsync(new RoundPage(race, 1));
            }
            else if (race.roundTwo == false)
            {
                await Navigation.PushAsync(new RoundPage(race, 2));
            }
            else if (race.roundThree == false)
            {
                await Navigation.PushAsync(new RoundPage(race, 3));
            }
            else if (race.Final == false)
            {
                await Navigation.PushAsync(new RoundPage(race, 4));
            }
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
                await DisplayAlert("Error","Cannot find table.","OK");
            }
            return null;
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

        private async void raceRecord_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRace = raceRecord.SelectedItem as Race;
            var choose = await DisplayAlert("Alert","Your race is incomplete.\nWould you like to continue race?","Yes", "No");
            if (choose)
            {
                CheckRaceStatus(_selectedRace);
            }
            else
            {
                return;
            }
        }
    }
}