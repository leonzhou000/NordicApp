﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NordicApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void addRace_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateRace());
        }

        private void rmRace_Clicked(object sender, EventArgs e)
        {

        }
    }
}