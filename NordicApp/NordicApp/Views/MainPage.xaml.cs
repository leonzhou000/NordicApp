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
        private ObservableCollection<Races> _races;
        private Races _selectedRace;

        public MainPage()
        {
            Init();
            InitializeComponent();
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
                await _connection.CreateTableAsync<Races>();
                _races = await getRaces();
                raceRecord.ItemsSource = _races;
            }
            catch
            {
                await DisplayAlert("Error", "Invalid create", "OK");
            }
            base.OnAppearing();
        }

        private async Task<ObservableCollection<Races>> getRaces()
        {
            try
            {
                var racelist = await _connection.Table<Races>().ToListAsync();
                return new ObservableCollection<Races>(racelist);
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
                var race = raceRecord.SelectedItem as Races;
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
            _selectedRace = raceRecord.SelectedItem as Races;
            var choose = await DisplayAlert("Alert","Your race is incomplete.\nWould you like to continue race?","Yes", "No");
            if (choose)
            {
                await Navigation.PushAsync(new DisplayRacers(_selectedRace));
            }
            else
            {
                return;
            }
        }
    }
}