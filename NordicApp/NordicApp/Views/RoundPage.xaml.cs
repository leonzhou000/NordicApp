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
    public partial class RoundPage : ContentPage
    {
        public RoundPage(Races race)
        {
            InitializeComponent();
        }
    }
}