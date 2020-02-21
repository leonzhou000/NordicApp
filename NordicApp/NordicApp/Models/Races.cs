using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace NordicApp.Models
{
    public class Races : INotifyPropertyChanged
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

        [MaxLength(255)]
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


        [MaxLength(255)]
        public DateTime addDate { get; set; }

        [MaxLength(255)]
        public DateTime updateData { get; set; }

        public bool Selected { get; set; }

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
