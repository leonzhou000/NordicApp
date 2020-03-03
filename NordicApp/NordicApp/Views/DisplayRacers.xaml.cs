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
    public partial class DisplayRacers : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Racers> _racers;
        private Races _raceInfo;
        private Racers _selectedRacer;

        public DisplayRacers(Races race)
        {
            _raceInfo = race;
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
                await _connection.CreateTableAsync<Racers>();
                _racers = await getRaces();
                racersList.ItemsSource = _racers;
            }
            catch
            {
                await DisplayAlert("Error", "Invalid create", "OK");
            }
            base.OnAppearing();
        }

        private async Task<ObservableCollection<Racers>> getRaces()
        {
            try
            {
                var table = await _connection.Table<Racers>().ToListAsync();
                var racers = from people in table
                             where people.dataset == _raceInfo.Id 
                             orderby people.Number ascending
                             select people;
                return new ObservableCollection<Racers>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private async void addRacer_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateRacers(_raceInfo));
        }

        private async void deleteRacer_Clicked(object sender, EventArgs e)
        {
            try
            {
                var racer = racersList.SelectedItem as Racers;
                if (racer == null)
                {
                    await DisplayAlert("Alert", "No item selected", "OK");
                    return;
                }
                await _connection.DeleteAsync(racer);
                _racers.Remove(racer);
            }
            catch
            {
                await DisplayAlert("Error", "Fail to delete race.", "OK");
                return;
            }
        }

        private void Finshed_Clicked(object sender, EventArgs e)
        {

        }

        private void racer_View_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            
        }

        private void racer_View_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private void modifyRacer_Clicked(object sender, EventArgs e)
        {

        }

        private void Start_time_Clicked(object sender, EventArgs e)
        {

        }

        private void Stop_time_Clicked(object sender, EventArgs e)
        {

        }
    }
}