﻿using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NordicApp.Models
{
    public class Race : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _Name;

        [MaxLength(255)]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) { return; }
                _Name = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _style;

        public string Style
        {
            get { return _style; }
            set
            {
                if (_style == value) { return; }
                _style = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Style));
            }
        }

        public DateTime addDate { get; set; }

        public bool Prelimary { get; set; }

        public bool roundOne { get; set; }
        
        public bool roundTwo { get; set; }
        
        public bool roundThree { get; set; }
        
        public bool Final { get; set; }

        public bool Selected { get; set; }

        public bool getRoundStatus(int round)
        {
            switch (round)
            {
                case (0):
                    return Prelimary;
                case (1):
                    return roundOne;
                case (2):
                    return roundTwo;
                case (3):
                    return roundThree;
                case (4):
                    return Final;
                default:
                    return true;
            }
        }

        public void setRoundStatus(int round)
        {
            switch (round)
            {
                case (1):
                    roundOne = true;
                    return;
                case (2):
                    roundTwo = true;
                    return;
                case (3):
                    roundThree = true;
                    return;
                case (4):
                    Final = true;
                    return;
                default:
                    Prelimary = true;
                    return;
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
