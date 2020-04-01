using System;
using SQLite;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NordicApp.Models
{
    public class Racers : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _FirstName;

        [MaxLength(255)]
        public string Fname
        {
            get { return _FirstName; }
            set
            {
                if (_FirstName == value) { return; }
                _FirstName = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Fname));
            }
        }

        private string _LastName;

        [MaxLength(255)]
        public string Lname
        {
            get { return _LastName; }
            set
            {
                if (_LastName == value) { return; }
                _LastName = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Lname));
            }
        }

        public int Number { get; set; }

        private string _ageGroup;

        [MaxLength(255)]
        public string ageGroup
        {
            get { return _ageGroup; }
            set
            {
                if (_ageGroup == value) { return; }
                _ageGroup = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ageGroup));
            }
        }

        private int _heatNumber;

        public int heatNumber
        {
            get { return _heatNumber; }
            set
            {
                if (_heatNumber == value) { return; }
                _heatNumber = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(heatNumber));
            }
        }

        public int roundOnePlacement { get; set; }
        
        public int roundTwoPlacement { get; set; }
        
        public int roundThreePlacement { get; set; }

        public int roundFinalPlacement { get; set; }

        public bool premlStarted { get; set; }

        public bool premlFinished { get; set; }

        public bool roundOneStart { get; set; }

        public bool roundOneFinish { get; set; }

        public bool roundTwoStart { get; set; }

        public bool roundTwoFinsh { get; set; }

        public bool roundThreeStart { get; set; }

        public bool roundThreeFinsh { get; set; }

        public bool roundFinalStart { get; set; }
        
        public bool roundFinalEnd { get; set; }

        public bool disqualified { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public int dataset { get; set; }

        public bool Selected { get; set; }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}