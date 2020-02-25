using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateRacers : ContentPage
    {
        private SQLiteAsyncConnection _connection;

        public CreateRacers()
        {
            InitializeComponent();
        }

        private void Done_Clicked(object sender, EventArgs e)
        {

        }

        private void addRacer_Clicked(object sender, EventArgs e)
        {
<<<<<<< Updated upstream

=======
            Navigation.PushModalAsync(new Page());
>>>>>>> Stashed changes
        }
    }
}