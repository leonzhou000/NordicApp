using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NordicApp.Data;
using NordicApp.Models;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRacers : ContentPage
    {
        private SQLiteAsyncConnection _connection;
        private Race _raceInfo;
        private List<Racer> _raceList;

        public CreateRacers(Race race)
        {
            InitializeComponent();
            Init(race);
        }

        private async void Init(Race race)
        {
            try 
            { 
                _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            }
            catch 
            {
                await DisplayAlert("Error", "SQL Table Connection", "OK");
                return;
            }

            try
            {
                await _connection.CreateTableAsync<Racer>();
            }
            catch
            {
                await DisplayAlert("ERROR", "Failed to add Racer.", "OK");
                return;
            }

            _raceInfo = race;
            _raceList = await getRacersList();
        }

        private async Task<List<Racer>> getRacersList()
        {
            var table = await _connection.Table<Racer>().ToListAsync();
            var racers = from people in table
                         where people.dataset == _raceInfo.Id
                         select people;
            return new List<Racer>(racers);
        }

        private bool infoChecker()
        {
            if (String.IsNullOrEmpty(firstName.Text))
            {
                DisplayAlert("Text empty.", "Please enter a first name.", "OK");
                return false;
            }

            if (String.IsNullOrEmpty(lastName.Text))
            {
                DisplayAlert("Text empty.", "Please enter a last name.","OK");
                return false;
            }
            
            if (String.IsNullOrEmpty(bibNumber.Text))
            {
                DisplayAlert("No bib number.","Please enter a bib number.","OK");
                return false;
            }

            return true;
        }

        private bool checkForExistingRacer()
        {
            if (_raceList.Count == 0)
                return true;

            for(int i=0; i<_raceList.Count; i++)
            {
                if(_raceList[i].Number == getBibNumber())
                {
                    DisplayAlert("Dupicate Racer","You have already use this bib number.\nPlease enter another bib number","Ok");
                    return false;
                }
            }

            return true;
        }

        private int getBibNumber()
        {
            return Int32.Parse(bibNumber.Text);
        }

        private async void addRacer_Clicked(object sender, EventArgs e)
        {
            TimeSpan _default = new TimeSpan();
            if (infoChecker() && checkForExistingRacer())
            {
                var racer = new Racer()
                {
                    Fname = firstName.Text,
                    Lname = lastName.Text,
                    Number = getBibNumber(),
                    ageGroup = Group.Text,
                    dataset = _raceInfo.Id,
                    Ranking = 0,
                    status = Status.StandyBy.ToString(),
                    StartTime = _default,
                    EndTime = _default,
                    ElapsedTime = _default,
                    roundOnePlacement = -1,
                    roundOneLane = -1,
                    roundTwoPlacement = -1,
                    roundTwoLane = -1,
                    roundThreePlacement = -1,
                    roundThreeLane = -1,
                    finalsPlacement = -1,
                    finalLane = -1,
                    premlStarted = false,
                    premlFinished = false,
                    roundOneFinish = false,
                    roundTwoFinish = false,
                    roundThreeFinish = false,
                    finalsFinish = false,
                    disqualified = false,
                    Selected = false
                };
                await _connection.InsertAsync(racer);
                await DisplayAlert("Racer added", "Your racer was added.", "OK");
            }
            _raceList = await getRacersList();
            firstName.Text = "";
            lastName.Text = "";
            Group.Text = "";
            bibNumber.Text = "";
            return;
        }

        private async void doneBtm_Clicked(object sender, EventArgs e)
        {
            var done = await DisplayAlert("Racer Added", "Are you finished?", "Yes", "No");
            if (done)
            {
                await Navigation.PopAsync();
            }

            return;
        }
    }
}