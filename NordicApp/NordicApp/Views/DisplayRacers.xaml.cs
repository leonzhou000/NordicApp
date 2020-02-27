using NordicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayRacers : ContentPage
    {
        public DisplayRacers(Races race)
        {
            InitializeComponent();
        }

        private void StartRace(object sender, EventArgs e)
        {
            /*
             *  Transition to the PRELIM round  
             */
        }

        private async void addRacer_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateRacers());
        }

        private void deleteRacer_Clicked(object sender, EventArgs e)
        {

        }

        private void Finshed_Clicked(object sender, EventArgs e)
        {

        }
    }
}