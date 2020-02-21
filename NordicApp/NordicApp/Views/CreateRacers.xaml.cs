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
    public partial class CreateRacers : ContentPage
    {
        public CreateRacers()
        {
            InitializeComponent();
        }

        private void AddRacer(object sender, EventArgs e)
        {
            /**
             * Pull data from tables and add to the database
             * 
             */
        }
        private void DonePressed(object sender, EventArgs e)
        {
            Navigation.PushModalAsync
        }
    }
}