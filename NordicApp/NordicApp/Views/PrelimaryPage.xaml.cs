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
        private ObservableCollection<Racer> _racers;
        private Race _raceInfo;
        private int _round = 0;
        private Racer _selectedRacer;
        Stopwatch _stopwatch;

        public PrelimaryPage(Race race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Race race)
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
                await _connection.CreateTableAsync<Racer>();
                _racers = await getRacers();
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

        private async Task<ObservableCollection<Racer>> getRacers()
        {
            try
            {
                var table = await _connection.Table<Racer>().ToListAsync();
                var racers = from people in table
                             where people.dataset == _raceInfo.Id 
                             orderby people.Number ascending
                             select people;
                return new ObservableCollection<Racer>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private TimeSpan getElapsedTime(Racer racer)
        {
            return racer.EndTime.Subtract(racer.StartTime);
        }

        private TimeSpan RecordTime()
        {
            return _stopwatch.Elapsed;
        }

        private async void UpdateInfo(Racer racer)
        {
            await _connection.UpdateAsync(racer);
            _racers = await getRacers();
            racersList.ItemsSource = _racers;
        }

        private void checkRacerStatus()
        {
            foreach (Racer racer in _racers)
            {
                if (racer.premlFinished == false)
                {
                    racer.disqualified = true;
                }
            }
            _connection.UpdateAllAsync(_racers);
        }

        private void modifyRacer_Clicked(object sender, EventArgs e)
        {

        }

        private void Start_time_Clicked(object sender, EventArgs e)
        {
            _stopwatch.Start();
            addRacer.IsEnabled = false;
            modifyRacer.IsEnabled = false;
            deleteRacer.IsEnabled = false;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                timer.Text = _stopwatch.Elapsed.ToString("hh\\:mm\\:ss");
                return true;
            });
        }

        private void Stop_time_Clicked(object sender, EventArgs e)
        {
            _stopwatch.Stop();
            addRacer.IsEnabled = true;
            modifyRacer.IsEnabled = true;
            deleteRacer.IsEnabled = true;
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
            _stopwatch.Stop();
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

        private async void Finshed_Clicked(object sender, EventArgs e)
        {
            checkRacerStatus();
            var choose = await DisplayAlert("Finish.", "Are you done with the Prelimary Round?", "Yes", "No");
            if (choose)
            {
                _stopwatch.Stop();
                //_raceInto.setRoundStatus(_round);
                _round++;
                await _connection.UpdateAsync(_raceInfo);
                await Navigation.PushAsync(new RoundPage(_raceInfo, _round));
            }
            else
            {
                return;
            }
        }

        private void racer_View_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _selectedRacer = racersList.SelectedItem as Racer;
            if ( _selectedRacer.premlStarted == false && _stopwatch.IsRunning)
            {
                _selectedRacer.premlStarted = true;
                _selectedRacer.StartTime = RecordTime();
                UpdateInfo(_selectedRacer);
                racersList.SelectedItem = null;
                return;
            }

            if ( _selectedRacer.premlStarted && _selectedRacer.premlFinished == false && _stopwatch.IsRunning)
            {
                _selectedRacer.EndTime = RecordTime();
                _selectedRacer.premlFinished = true;
                _selectedRacer.ElapsedTime = getElapsedTime(_selectedRacer);
                UpdateInfo(_selectedRacer);
                racersList.SelectedItem = null;
                return;
            }

            racersList.SelectedItem = null;
            return;
        }
    }
}