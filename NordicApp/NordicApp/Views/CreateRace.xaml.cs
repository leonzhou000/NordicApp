using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SQLite;
using NordicApp.Models;
using NordicApp.Data;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRace : ContentPage
    {

        private SQLiteAsyncConnection _connection;
        private List<Race> _Races;
        private DateTime _dateSelected;

        public CreateRace()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            try 
            {
                _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            }
            catch
            { 
                await DisplayAlert("Error", "SQL Table Connection.", "OK"); 
            }
            
            GetRacesTable();
        }

        private async void GetRacesTable()
        {
            try
            {
                var races = await _connection.Table<Race>().ToListAsync();
                _Races = new List<Race>(races);
            }
            catch
            {
                await DisplayAlert("Error", "Can not find table", "Ok");
                return;
            }
        }

        private bool checkRacesInfo()
        {
            if (String.IsNullOrEmpty(raceName.Text))
            {
                DisplayAlert("Name Blank","Please Enter a name.","OK");
                return false;
            }
            if (raceStyle.SelectedItem == null)
            {
                DisplayAlert("Choose racing style.", "Please Choose a race style.", "OK");
                return false;
            }
            return true;
        }

        private async void BackBtm_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void SubmitBtm_Clicked(object sender, EventArgs e)
        {
            if (checkRacesInfo())
            {
                var race = new Race()
                {
                    Name = raceName.Text,
                    Style = raceStyle.SelectedItem.ToString(),
                    addDate = _dateSelected,
                    Prelimary = false,
                    roundOne = false,
                    roundTwo = false,
                    roundThree = false,
                    Final = false,
                    Selected = false
                };

                try
                {
                    await _connection.InsertAsync(race);
                    await Navigation.PushAsync(new PrelimaryPage(race));
                }
                catch
                {
                    await DisplayAlert("Error","Failed to add item to table","OK");
                    return;
                }
            }
        }

        private void raceDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            _dateSelected = e.NewDate;
        }
    }
}