using System;
using SQLite;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace NordicApp.Models
{
    class RacerGroups : ObservableCollection<Racer>
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }

        public RacerGroups(string title, string shortTitle)
        {
            Title = title;
            ShortTitle = shortTitle;
        }
    }

    public class Racer : INotifyPropertyChanged
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

        private string _gender;

        public string gender
        {
            get { return _gender; }
            set
            {
                if (_gender == value) { return; }
                _gender = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(_gender));
            }
        }

        public int getRoundHeatNumber(int round)
        {
            switch (round)
            {
                case (1):
                    return roundOneHeatNumber;
                case (2):
                    return roundTwoHeatNumber;
                case (3):
                    return roundThreeHeatNumber;
                case (4):
                    return finalsHeatNumber;
                default:
                    return 0;
            }
        }

        public void setRecordHeat(int round, int heat)
        {
            switch (round)
            {
                case (1):
                    roundOneHeatNumber = heat;
                    return;
                case (2):
                    roundTwoHeatNumber = heat;
                    return;
                case (3):
                    roundThreeHeatNumber = heat;
                    return;
                case (4):
                    finalsHeatNumber = heat;
                    return;
                default:
                    return;
            }
        }

        public int Ranking{ get; set; }

        public string status { get; set; }

        public int roundOneHeatNumber { get; set; }

        public int roundOneFinish { get; set; } 
        
        public int roundTwoHeatNumber { get; set; }

        public int roundTwoFinish { get; set; }
        
        public int roundThreeHeatNumber { get; set; }

        public int roundThreeFinish { get; set; }

        public int finalsHeatNumber { get; set; }

        public int finalsFinish { get; set; }

        public bool premlStarted { get; set; }

        public bool premlFinished { get; set; }

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