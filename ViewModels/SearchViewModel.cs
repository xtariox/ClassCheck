using ClassCheck.Models;
using ClassCheck.Services;
using ClassCheck.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

public class SearchViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly MajorViewModel _majorViewModel;
    private readonly AddLessonViewModel _lessonViewModel;

    public ObservableCollection<Major> Majors => _majorViewModel.Majors;
    public ObservableCollection<Lesson> Lessons => _lessonViewModel.Lessons;
    public ObservableCollection<Attendance> AttendanceRecords { get; private set; } = new ObservableCollection<Attendance>();
    public ObservableCollection<Attendance> FilteredAttendance { get; private set; } = new ObservableCollection<Attendance>();

    public ICommand LoadSearchCommand { get; }
    public ICommand SaveSearchCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ReturnCommand { get; }

    // Search criteria properties
    private string _fullName;
    public string FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    private ObservableCollection<Lesson> _filteredLessons;
    public ObservableCollection<Lesson> FilteredLessons
    {
        get => _filteredLessons;
        set => SetProperty(ref _filteredLessons, value);
    }

    private Major _selectedMajor;
    public Major SelectedMajor
    {
        get => _selectedMajor;
        set
        {
            if (SetProperty(ref _selectedMajor, value))
            {
                // Filter lessons based on the selected major's name
                if (value != null)
                {
                    FilteredLessons = new ObservableCollection<Lesson>(Lessons.Where(lesson =>
                        lesson.Major.Equals(value.Name, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    // Reset to all lessons if no major is selected
                    FilteredLessons = new ObservableCollection<Lesson>(Lessons);
                }

                OnPropertyChanged(nameof(FilteredLessons)); // Notify the UI about the update
            }
        }
    }

    private DateTime? _scheduleDate; // Nullable DateTime for optional date filter
    public DateTime? ScheduleDate
    {
        get => _scheduleDate;
        set => SetProperty(ref _scheduleDate, value);
    }

    private Lesson _selectedLesson;
    public Lesson SelectedLesson
    {
        get => _selectedLesson;
        set => SetProperty(ref _selectedLesson, value);
    }

    private bool _isSearchLoaded;
    public bool IsSearchLoaded
    {
        get => _isSearchLoaded;
        set => SetProperty(ref _isSearchLoaded, value);
    }

    // Success and Error Messages
    private string _successMessage;
    public string SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _isSuccessVisible;
    public bool IsSuccessVisible
    {
        get => _isSuccessVisible;
        set => SetProperty(ref _isSuccessVisible, value);
    }

    private bool _isErrorVisible;
    public bool IsErrorVisible
    {
        get => _isErrorVisible;
        set => SetProperty(ref _isErrorVisible, value);
    }

    public SearchViewModel(DatabaseService databaseService, MajorViewModel majorViewModel, AddLessonViewModel lessonViewModel)
    {
        _databaseService = databaseService;
        _majorViewModel = majorViewModel;
        _lessonViewModel = lessonViewModel;

        LoadSearchCommand = new Command(async () =>
        {
            IsSearchLoaded = false; // Reset the search results
            await PerformSearchAsync();
            IsErrorVisible = false; // Hide any previous error messages
            IsSearchLoaded = true; // Show the search results
        }
        );
        SaveSearchCommand = new Command(async () => await SaveSearchAsync());
        CancelCommand = new Command(() =>
        {
            ClearFields();
            Shell.Current.GoToAsync("///home");
        });

        ReturnCommand = new Command(() => {
            System.Diagnostics.Debug.WriteLine("Return button clicked");

            // Reset the search state
            IsSearchLoaded = false; // Hide the search results
            FilteredAttendance.Clear(); // Clear the filtered results

            // Navigate back to the search page
            Shell.Current.GoToAsync("///search");
        });

        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        await _majorViewModel.LoadMajorsAsync();
        await _lessonViewModel.LoadLessonsAsync();

        AttendanceRecords = new ObservableCollection<Attendance>(await _databaseService.GetAllAsync<Attendance>());
        FilteredAttendance = new ObservableCollection<Attendance>(AttendanceRecords);

        // Initialize FilteredLessons with all lessons
        FilteredLessons = new ObservableCollection<Lesson>(Lessons);

        OnPropertyChanged(nameof(Lessons));
        OnPropertyChanged(nameof(Majors));
        OnPropertyChanged(nameof(FilteredAttendance));
        OnPropertyChanged(nameof(FilteredLessons)); // Ensure the UI is aware of the lessons initially
    }

    private async Task PerformSearchAsync()
    {
        try
        {
            // Start with all attendance records
            var filtered = AttendanceRecords.AsEnumerable();

            // Apply filters safely
            if (string.IsNullOrWhiteSpace(FullName) || SelectedMajor == null)
            {
                // Display an error message or exit the search as these fields are mandatory
                ErrorMessage = "Full name and Major are required.";
                IsErrorVisible = true;
                return; // Exit if these fields are not filled
            }

            // Split FullName into parts (first name, last name)
            var nameParts = FullName.Split(' ');

            // Filter by name
            filtered = filtered.Where(a =>
                !string.IsNullOrEmpty(a.StudentFName) && !string.IsNullOrEmpty(a.StudentLName) &&
                nameParts.Any(part => a.StudentFName.Contains(part, StringComparison.OrdinalIgnoreCase) ||
                                      a.StudentLName.Contains(part, StringComparison.OrdinalIgnoreCase))
            );

            // Filter by Major
            if (SelectedMajor != null)
            {
                filtered = filtered.Where(a =>
                    a.Major?.Equals(SelectedMajor.Name, StringComparison.OrdinalIgnoreCase) == true);
            }

            // Filter by Date (for example, if you want to filter by a specific date)
            if (ScheduleDate.HasValue)
            {
                filtered = filtered.Where(a => a.AttendanceDate.Date == ScheduleDate.Value.Date); // Exact date match
            }

            // Update FilteredAttendance collection
            FilteredAttendance = new ObservableCollection<Attendance>(filtered);
            IsSearchLoaded = FilteredAttendance.Any();

            // Debug: Log the number of results
            System.Diagnostics.Debug.WriteLine($"Filtered {FilteredAttendance.Count} attendance records.");

            // Notify the UI
            OnPropertyChanged(nameof(FilteredAttendance));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error performing search: {ex.Message}";
            IsErrorVisible = true;
        }
    }

    private async Task SaveSearchAsync()
    {
        try
        {
            if (FilteredAttendance.Any())
            {
                int updatedCount = 0;
                foreach (var attendance in FilteredAttendance)
                {
                    var result = await _databaseService.UpdateAttendanceAsync(attendance);
                    updatedCount += result; // Assuming `result` returns 1 for successful update
                }

                if (updatedCount > 0)
                {
                    SuccessMessage = $"{updatedCount} attendance records saved successfully!";
                    IsSuccessVisible = true;
                }
                else
                {
                    ErrorMessage = "Failed to save search results.";
                    IsErrorVisible = true;
                }
            }
            else
            {
                ErrorMessage = "No search results to save.";
                IsErrorVisible = true;
            }
            ClearFields();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving search results: {ex.Message}";
            IsErrorVisible = true;
        }
    }


    private void ClearFields()
    {
        FullName = string.Empty;
        SelectedMajor = null;
        SelectedLesson = null;
        IsSearchLoaded = false;
        ScheduleDate = DateTime.Today;
        FilteredAttendance.Clear();
        OnPropertyChanged(nameof(FilteredAttendance));
    }
}
