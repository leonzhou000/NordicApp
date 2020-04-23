using System;
using SQLite;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace NordicApp.Models
{
    public enum Status
    {
        StandyBy,
        Started,
        Finished,
        DQ,
        Premils,
        One,
        Two,
        Three    
    }

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

        public int Ranking { get; set; }

        public string status { get; set; }

        public int roundOneHeatNumber { get; set; }

        public int roundOnePlacement { get; set; }

        public bool roundOneFinish { get; set; }

        public int roundTwoHeatNumber { get; set; }
        
        public int roundTwoPlacement { get; set; }

        public bool roundTwoFinish { get; set; }
        
        public int roundThreeHeatNumber { get; set; }

        public int roundThreePlacement { get; set; }

        public bool roundThreeFinish { get; set; }

        public int finalsHeatNumber { get; set; }
        
        public int finalsPlacement { get; set; }

        public bool finalsFinish { get; set; }

        public bool premlStarted { get; set; }

        public bool premlFinished { get; set; }

        public bool disqualified { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan ElapsedTime { get; set; }

        public int dataset { get; set; }

        public bool Selected { get; set; }

        public void setHeatNumber(int round, int heat)
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

        public int getHeatNumber(int round)
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

        public void setPlacement(int round, int placement)
        {
            switch (round)
            {
                case (1):
                    roundOnePlacement = placement;
                    return;
                case (2):
                    roundTwoPlacement = placement;
                    return;
                case (3):
                    roundThreePlacement = placement;
                    return;
                case (4):
                    finalsPlacement = placement;
                    return;
                default:
                    return;
            }
        }

        public int getRoundPlacement(int round)
        {
            switch (round)
            {
                case (1):
                    return roundOnePlacement;
                case (2):
                    return roundTwoPlacement;
                case (3):
                    return roundThreePlacement;
                case (4):
                    return finalsPlacement;
                default:
                    return 0;
            }
        }

        public void setRoundFinish(int round)
        {
            switch (round)
            {
                case (1):
                    roundOneFinish = true;
                    return;
                case (2):
                    roundTwoFinish = true;
                    return;
                case (3):
                    roundThreeFinish = true;
                    return;
                case (4):
                    finalsFinish = true;
                    return;
                default:
                    premlFinished = true;
                    return;
            }
        }

        public bool getRoundFinish(int round)
        {
            switch (round)
            {
                case (1):
                    return roundOneFinish;
                case (2):
                    return roundTwoFinish; 
                case (3):
                    return roundThreeFinish;
                case (4):
                    return finalsFinish; 
                default:
                    return premlFinished;
            }
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}