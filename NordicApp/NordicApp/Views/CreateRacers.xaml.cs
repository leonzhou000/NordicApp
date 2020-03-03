using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Races _raceInfo;

        public CreateRacers(Races race)
        {
            Init(race);
            InitializeComponent();
        }

        private async void Init(Races race)
        {
            _raceInfo = race;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
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

        private int getBibNumber()
        {
            return Int32.Parse(bibNumber.Text);
        }

        private async void Done_Clicked(object sender, EventArgs e)
        {
            var done = await DisplayAlert("Racer Added", "Are you finished", "Yes", "No");
            if (done)
            {
                await Navigation.PopAsync();
            }
            return;
        }

        private async void addRacer_Clicked(object sender, EventArgs e)
        {
            if (infoChecker())
            {
                try
                {
                    await _connection.CreateTableAsync<Racers>();
                    var racer = new Racers()
                    {
                        Fname = firstName.Text,
                        Lname = lastName.Text,
                        Number = getBibNumber(),
                        ageGroup = Group.Text,
                        dataset = _raceInfo.Id,
                        started = false,
                        finished = false,
                        Selected = false
                    };
                    await _connection.InsertAsync(racer);
                }
                catch
                {
                    await DisplayAlert("ERROR","Failed to add Racer.","OK");
                    return;
                }
            }
            return;
        }
    }
}