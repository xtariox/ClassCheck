using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClassCheck.Models
{
    public class Lesson : INotifyPropertyChanged, IEntity
    {
        private int _id;
        private string _courseName;
        private int _majorId; // Use MajorId as foreign key
        private DateTime _schedule;
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
        public int MajorId
        {
            get => _majorId;
            set
            {
                _majorId = value;
                OnPropertyChanged();
            }
        }

        [Ignore]
        public Major Major { get; set; } // Ignored by SQLite, used for binding

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

        public Lesson()
        {
            // Remove this line as the professor will be set in the ViewModel
            // Professor = Environment.UserName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
