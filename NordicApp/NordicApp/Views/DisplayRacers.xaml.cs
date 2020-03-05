using NordicApp.Models;
using System;
using System.Diagnostics;
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
        Stopwatch _stopwatch;

        public DisplayRacers(Races race)
        {
            _raceInfo = race;
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            _stopwatch = new Stopwatch();
            timer.Text = "00:00:00";
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
                if (_selectedRacer == null)
                {
                    await DisplayAlert("Alert", "No item selected", "OK");
                    return;
                }
                await _connection.DeleteAsync(_selectedRacer);
                _racers.Remove(_selectedRacer);
            }
            catch
            {
                await DisplayAlert("Error", "Fail to delete race.", "OK");
                return;
            }
        }

        private async void Finshed_Clicked(object sender, EventArgs e)
        {
            var choose = await DisplayAlert("Alert", "Your race is incomplete.\nWould you like to continue race?", "Yes", "No");
            if (choose)
            {
                await Navigation.PushAsync(new ResultsPage(_raceInfo));
            }
            else
            {
                return;
            }
        }

        private async void racer_View_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRacer = racersList.SelectedItem as Racers;
            if ( _selectedRacer.started == false && _selectedRacer.finished == false)
            {
                TimeSpan startTime = _stopwatch.Elapsed;
                _selectedRacer.StartTime = startTime;
                _selectedRacer.started = true;
                await _connection.UpdateAsync(_selectedRacer);
                _racers = await getRaces();
                racersList.ItemsSource = _racers;
                racersList.SelectedItem = null;
                return;
            }
            if ( _selectedRacer.finished == false && _selectedRacer.started == true)
            {
                TimeSpan endTime = _stopwatch.Elapsed;
                _selectedRacer.EndTime = endTime;
                _selectedRacer.finished = true;
                await _connection.UpdateAsync(_selectedRacer);
                _racers = await getRaces();
                racersList.ItemsSource = _racers;
                racersList.SelectedItem = null;
                return;
            }
            racersList.SelectedItem = null;
            return;
        }

        private void modifyRacer_Clicked(object sender, EventArgs e)
        {

        }

        private void Start_time_Clicked(object sender, EventArgs e)
        {
            _stopwatch.Start();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                timer.Text = _stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                return true;
            });
        }

        private void Stop_time_Clicked(object sender, EventArgs e)
        {
            _stopwatch.Stop();
        }

        private void Reset_time_Clicked(object sender, EventArgs e)
        {
            _stopwatch.Restart();
            _stopwatch.Stop();
        }
    }
}