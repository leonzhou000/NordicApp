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
    public partial class PrelimaryPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private ObservableCollection<Racers> _racers;
        private Races _raceInfo;
        private Racers _selectedRacer;
        Stopwatch _stopwatch;

        public PrelimaryPage(Races race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Races race)
        {
            _raceInfo = race;
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

        protected override bool OnBackButtonPressed()
        {
            return true;
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

        private TimeSpan getElapsedTime(Racers racer)
        {
            return racer.EndTime.Subtract(racer.StartTime);
        }

        private TimeSpan RecordTime()
        {
            return _stopwatch.Elapsed;
        }

        private async void UpdateInfo(Racers racer)
        {
            await _connection.UpdateAsync(racer);
            _racers = await getRaces();
            racersList.ItemsSource = _racers;
            racersList.SelectedItem = null;
        }

        private async void checkRacerStatus()
        {
            foreach (Racers racer in _racers)
            {
                if (racer.EndTime == null)
                {
                    racer.disqualified = true;
                    await _connection.UpdateAsync(racer);
                }
            }
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
                    await DisplayAlert("Alert", "No item selected.", "OK");
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

        private async void exitRace_Clicked(object sender, EventArgs e)
        {
            var choose = await DisplayAlert("Exit", "Your rac eis not finished.\n Would like to leave?", "Yes", "No");
            if (choose)
            {
                await Navigation.PushAsync(new MainPage());
            }
            else
            {
                return;
            }
        }

        private async void Finshed_Clicked(object sender, EventArgs e)
        {
            _stopwatch.Stop();
            checkRacerStatus();
            var choose = await DisplayAlert("Finish.", "Are you done with the Prelimary Round?", "Yes", "No");
            if (choose)
            {
                _raceInfo.Prelimary = true;
                await _connection.UpdateAsync(_raceInfo);
                await Navigation.PushAsync(new ResultsPage(_raceInfo));
            }
            else
            {
                return;
            }
        }

        private void racer_View_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRacer = racersList.SelectedItem as Racers;
            if ( _selectedRacer.premlStarted == false && _selectedRacer.premlFinished == false && _stopwatch.IsRunning)
            {
                _selectedRacer.StartTime = RecordTime();
                _selectedRacer.premlStarted = true;
                UpdateInfo(_selectedRacer);
                return;
            }
            if ( _selectedRacer.premlFinished == false && _selectedRacer.premlStarted == true)
            {
                _selectedRacer.EndTime = RecordTime();
                _selectedRacer.premlFinished = true;
                _selectedRacer.ElapsedTime = getElapsedTime(_selectedRacer);
                UpdateInfo(_selectedRacer);
                return;
            }
            racersList.SelectedItem = null;
            return;
        }
    }
}