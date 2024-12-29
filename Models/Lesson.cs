using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.Models
{
    public class Lesson : INotifyPropertyChanged
    {
        private int _id;
        private string _courseName;
        private string _major;
        private DateTime _schedule; // Updated type to DateTime
        private string _professor;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string CourseName
        {
            get => _courseName;
            set
            {
                _courseName = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string Major
        {
            get => _major;
            set
            {
                _major = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public DateTime Schedule
        {
            get => _schedule;
            set
            {
                _schedule = value;
                OnPropertyChanged();
            }
        }

        [NotNull]
        public string Professor
        {
            get => _professor;
            set
            {
                _professor = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
