using System.Windows.Input;
using ClassCheck.Models;
using ClassCheck.Services;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

namespace ClassCheck.ViewModels
{
    public class AddLessonViewModel : BaseViewModel
    {
        public event EventHandler LessonAdded = delegate { };
        private readonly DatabaseService _databaseService;
        private readonly MajorViewModel _majorViewModel;
        public ObservableCollection<Major> Majors => _majorViewModel.Majors;

        private ObservableCollection<Lesson> _lessons = [];
        public ObservableCollection<Lesson> Lessons
        {
            get => _lessons;
            private set
            {
                _lessons = value;
                OnPropertyChanged();
            }
        }

        // Properties for binding
        private string _courseName;
        public string CourseName
        {
            get => _courseName;
            set => SetProperty(ref _courseName, value);
        }

        private string _professor;
        public string Professor
        {
            get => _professor;
            set => SetProperty(ref _professor, value);
        }

        private DateTime _schedule;
        public DateTime Schedule
        {
            get => _schedule;
            set => SetProperty(ref _schedule, value);
        }

        private Major _major;
        public Major Major
        {
            get => _major;
            set => SetProperty(ref _major, value);
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        // Commands for Add and Cancel actions
        public ICommand AddLessonCommand { get; }
        public ICommand CancelCommand { get; }

        public AddLessonViewModel(
            DatabaseService databaseService,
            MajorViewModel majorViewModel,
            ILogger<AddLessonViewModel> logger) : base(logger)
        {
            _databaseService = databaseService;
            _majorViewModel = majorViewModel;

            // Ensure majors are loaded asynchronously
            LoadMajorsAsync().ConfigureAwait(false);

            // Initialize the commands
            AddLessonCommand = new Command(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Add button clicked");
                await AddLessonAsync();
            });

            CancelCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("Cancel button clicked");
                Shell.Current.GoToAsync("///home");
            });
        }

        // Method to load majors asynchronously
        public async Task LoadMajorsAsync()
        {
            try
            {
                await _majorViewModel.LoadMajorsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading majors: {ex.Message}");
            }
        }

        // Method to add a lesson
        private async Task AddLessonAsync()
        {
            if (ValidateInput())
            {
                if (Schedule < DateTime.Now.Date)
                {
                    SuccessMessage = "The schedule date cannot be in the past.";
                    return;
                }

                // Create a new lesson
                var newLesson = new Lesson
                {
                    CourseName = CourseName,
                    Professor = Professor,
                    Schedule = Schedule,
                    Major = Major.Name
                };

                // Add to the database
                var result = await _databaseService.InsertAsync(newLesson);

                // Update the success or error message
                if (result > 0)
                {
                    SuccessMessage = "Lesson added successfully!";
                    ClearFields();
                    Lessons.Add(newLesson); // Update on main thread
                    OnPropertyChanged(nameof(Lessons)); // Notify that Lessons have been updated
                    LessonAdded?.Invoke(this, EventArgs.Empty); // Invoke the event
                }
                else
                {
                    SuccessMessage = "Failed to add the lesson.";
                }
            }
            else
            {
                SuccessMessage = "Please fill in all the required fields.";
            }
        }

        // Input validation
        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(CourseName) &&
                   !string.IsNullOrWhiteSpace(Professor) &&
                   Major != null;
        }

        // Reset the fields
        private void ClearFields()
        {
            CourseName = string.Empty;
            Professor = string.Empty;
            Schedule = DateTime.Now;
            Major = null;
        }

        // Method to load lessons asynchronously
        public async Task LoadLessonsAsync()
        {
            try
            {
                // Retrieve the list of lessons from the database
                var lessons = await _databaseService.GetAllAsync<Lesson>();

                // Clear the existing lessons collection
                Lessons.Clear();

                // Add each retrieved lesson to the Lessons collection
                foreach (var lesson in lessons)
                {
                    Lessons.Add(lesson);
                    System.Diagnostics.Debug.WriteLine($"Added lesson: {lesson.CourseName}");
                }

                // Debug the number of lessons loaded
                System.Diagnostics.Debug.WriteLine($"Total lessons loaded: {Lessons.Count}");
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur
                System.Diagnostics.Debug.WriteLine($"Error loading lessons: {ex.Message}");
            }
        }
    }
}
