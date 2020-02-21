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
        public DisplayRacers()
        {
            InitializeComponent();
        }

        private void StartRace(object sender, EventArgs e)
        {
            /*
             *  Transition to the PRELIM round
             */
        }
    }
}