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
    public partial class CreateRace : ContentPage
    {
        public CreateRace()
        {
            InitializeComponent();
        }

        private async void SubmitPressed(object sender, EventArgs e)
        {
            /*
             * Pull data from text and upload to database
             */
            await Navigation.PushModalAsync(new CreateRacers());
        }
    }
}