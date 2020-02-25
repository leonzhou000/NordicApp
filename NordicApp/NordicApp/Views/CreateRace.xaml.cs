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
        private List<Races> _Races;
        private DateTime SelectRaceDate;

        public CreateRace()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            try { _connection = DependencyService.Get<ISQLiteDb>().GetConnection(); }
            catch { await DisplayAlert("Error", "SQL Table Connection", "OK"); }
            GetRacesTable();
        }

        private async void GetRacesTable()
        {
            try
            {
                var races = await _connection.Table<Races>().ToListAsync();
                _Races = new List<Races>(races);
            }
            catch
            {
                await DisplayAlert("Error", "Can not find table", "Ok");
                return;
            }
        }

        private bool dateChecker()
        {
            return true;
        }

        private bool checkRacesInfo()
        {
            foreach(var race in _Races)
            {
                if (race.Name == raceName.Text & dateChecker())
                {
                    DisplayAlert("Alert","Race already exists","Ok");
                    return false;
                }
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
                await Navigation.PushAsync(new DisplayRacers());
            }
        }

        
    }
}