using NordicApp.Data;
using NordicApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultsPage : ContentPage
    {
        private Race _raceInfo;
        private SQLiteAsyncConnection _connection;
        private List<Racer> _racers;
        private int rank;

        public ResultsPage(Race race)
        {
            InitializeComponent();
            Init(race);

        }
        private async void Init(Race race)
        {
            _raceInfo = race;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            _racers = await getRacers();
            rank = _racers.Count;
            resultsViewer.ItemsSource = _racers;
        }

        private async Task<List<Racer>> getRacers()
        {
            try
            {
                var table = await _connection.Table<Racer>().ToListAsync();
                var racers = from people in table
                             where people.dataset == _raceInfo.Id
                             select people;

                return new List<Racer>(racers);
            }
            catch
            {
                await DisplayAlert("Error", "Cannot find table.", "OK");
                return null;
            }
        }

        private void doneBtm_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}