using ClassCheck.Services;
using ClassCheck.Models;
using ClassCheck.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq; // Ensure this is included

namespace ClassCheck.ViewModels
{
    public class AttendanceViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly MajorViewModel _majorViewModel;
        private readonly AddLessonViewModel _lessonViewModel;
        public ObservableCollection<Major> Majors => _majorViewModel.Majors;
        public ObservableCollection<Lesson> Lessons => _lessonViewModel.Lessons;

        private ObservableCollection<Lesson> _filteredLessons = new ObservableCollection<Lesson>();
        public ObservableCollection<Lesson> FilteredLessons
        {
            get => _filteredLessons;
            set => SetProperty(ref _filteredLessons, value);
        }

        public ObservableCollection<Attendance> Attendances { get; private set; } = new ObservableCollection<Attendance>();

        public ICommand LoadAttendanceCommand { get; }
        public ICommand CancelCommand { get; }

        private string _lessonId;
        public string LessonId
        {
            get => _lessonId;
            set => SetProperty(ref _lessonId, value);
        }

        private string _filiere;
        public string Filiere
        {
            get => _filiere;
            set => SetProperty(ref _filiere, value);
        }

        private DateTime _attendanceDate = DateTime.Today;
        public DateTime AttendanceDate
        {
            get => _attendanceDate;
            set => SetProperty(ref _attendanceDate, value);
        }

        private Lesson _selectedLesson;
        public Lesson SelectedLesson
        {
            get => _selectedLesson;
            set => SetProperty(ref _selectedLesson, value);
        }

        private Major _selectedMajor;
        public Major SelectedMajor
        {
            get => _selectedMajor;
            set
            {
                if (SetProperty(ref _selectedMajor, value))
                {
                    IsLessonPickerEnabled = _selectedMajor != null;
                    OnPropertyChanged(nameof(IsLessonPickerEnabled));
                    FilterLessons();
                }
            }
        }

        private bool _isLessonPickerEnabled;
        public bool IsLessonPickerEnabled
        {
            get => _isLessonPickerEnabled;
            set => SetProperty(ref _isLessonPickerEnabled, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isErrorVisible;
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        public AttendanceViewModel(DatabaseService databaseService, MajorViewModel majorViewModel, AddLessonViewModel lessonViewModel)
        {
            _databaseService = databaseService;
            _majorViewModel = majorViewModel;
            _lessonViewModel = lessonViewModel;

            CancelCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("Cancel button clicked");
                Shell.Current.GoToAsync("///home");
            });

            LoadAttendanceCommand = new Command(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Load attendance button clicked");
                await LoadAttendanceAsync();
            });

            // Ensure Majors and Lessons are loaded before setting the command
            LoadMajorsAndLessons();

            // Subscribe to the LessonAdded event
            _lessonViewModel.LessonAdded += async (sender, args) =>
            {
                await LoadMajorsAndLessons();
            };
        }

        public async Task LoadMajorsAndLessons()
        {
            await _majorViewModel.LoadMajorsAsync();
            await _lessonViewModel.LoadLessonsAsync();
            OnPropertyChanged(nameof(Majors)); // Notify that Majors have been updated
            OnPropertyChanged(nameof(Lessons)); // Notify that Lessons have been updated
            FilterLessons(); // Ensure lessons are filtered after loading
        }

        public void FilterLessons()
        {
            if (SelectedMajor != null)
            {
                System.Diagnostics.Debug.WriteLine($"Filtering lessons for major: {SelectedMajor.Name} (ID: {SelectedMajor.Id})");
                System.Diagnostics.Debug.WriteLine($"Total lessons before filtering: {Lessons.Count}");
                
                var filteredList = Lessons.Where(l => l.Major?.Id == SelectedMajor.Id).ToList();
                FilteredLessons = new ObservableCollection<Lesson>(filteredList);
                
                System.Diagnostics.Debug.WriteLine($"Filtered lessons count: {FilteredLessons.Count}");
                foreach (var lesson in FilteredLessons)
                {
                    System.Diagnostics.Debug.WriteLine($"Filtered lesson: {lesson.CourseName}, Major ID: {lesson.Major?.Id}");
                }
            }
            else
            {
                FilteredLessons.Clear();
                System.Diagnostics.Debug.WriteLine("Selected major is null, cleared filtered lessons");
            }
            OnPropertyChanged(nameof(FilteredLessons));
        }

        private async Task LoadAttendanceAsync()
        {
            if (SelectedLesson == null || SelectedMajor == null)
            {
                ErrorMessage = "Please select both a Major and a Lesson.";
                IsErrorVisible = true;
                return;
            }

            IsErrorVisible = false;
            Console.WriteLine($"Selected Lesson: {SelectedLesson.CourseName}, Selected Major: {SelectedMajor.Name}");

            try
            {
                var attendanceList = await _databaseService.GetAttendanceByFiltersAsync(
                    SelectedLesson.Id,
                    SelectedMajor.Name,
                    AttendanceDate
                );
                Attendances.Clear();

                foreach (var attendance in attendanceList)
                {
                    Attendances.Add(attendance);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading attendance: {ex.Message}");
            }
        }
    }
}
