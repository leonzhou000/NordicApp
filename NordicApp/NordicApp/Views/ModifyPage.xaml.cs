using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NordicApp.Data;
using NordicApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModifyPage : ContentPage
    {
        private Racer _racer;
        private Race _raceInfo;
        private int _round;
        private SQLiteAsyncConnection _connection;
        private List<Racer> _raceList;

        public ModifyPage(Race race,Racer racer, int round)
        {
            InitializeComponent();
            Init(race ,racer, round);
        }

        private async void Init(Race race, Racer racer, int round)
        {
            _racer = racer;
            _round = round;
            _raceInfo = race;
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            _raceList = await getRacersList();
            roundCheck(round);
            setInfo(racer);
        }

        private async Task<List<Racer>> getRacersList()
        {
            var table = await _connection.Table<Racer>().ToListAsync();
            var racers = from people in table
                         where people.dataset == _raceInfo.Id
                         select people;
            return new List<Racer>(racers);
        }

        private void roundCheck(int round)
        {
            if(round > 0)
            {
                placement.IsVisible = true;
                placementLabel.IsVisible = true;
            }
            else
            {
                placement.IsVisible = false;
                placementLabel.IsVisible = false;
            }
        }

        private void setInfo(Racer racer)
        {
            firstName.Text = racer.Fname;
            lastName.Text = racer.Lname;
            bibNumber.Text = racer.Number.ToString();
            placement.Text = (racer.getRoundPlacement(_round)+1).ToString();

            if(String.IsNullOrEmpty(racer.ageGroup) || String.IsNullOrEmpty(racer.gender))
            {
                gender.SelectedItem = null;
                ageGroup.SelectedItem = null;
            }
            else
            {
                gender.SelectedItem = racer.gender;
                ageGroup.SelectedItem = racer.ageGroup;
            }
        }

        private int getBibNumber()
        {
            return Int32.Parse(bibNumber.Text);
        }

        private int getPlacement()
        {
            return Int32.Parse(placement.Text);
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
                DisplayAlert("Text empty.", "Please enter a last name.", "OK");
                return false;
            }

            if (String.IsNullOrEmpty(bibNumber.Text))
            {
                DisplayAlert("No bib number.", "Please enter a bib number.", "OK");
                return false;
            }

            return true;
        }

        private async void changeRacerInfo(Racer racer)
        {
            if(infoChecker())
            {
                racer.Fname = firstName.Text;
                racer.Lname = lastName.Text;
                racer.gender = gender.SelectedItem.ToString();
                racer.ageGroup = ageGroup.SelectedItem.ToString();
                racer.Number = getBibNumber();
                racer.setPlacement(_round, getPlacement()-1);
                await _connection.UpdateAsync(racer);
                await DisplayAlert("Success.","Racer has been updated","Ok");
            }
            return;
        }

        private void doneBtm_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void updateRacerBtm_Clicked(object sender, EventArgs e)
        {
            var response = await DisplayAlert("Check","Are you sure all information enter is correct?","Yes","No");
            if (response)
            {
                changeRacerInfo(_racer);
            }
            else
            {
                await DisplayAlert("Failed.", "Racer failed to be updated", "Ok");
                return;
            }
        }
    }
}