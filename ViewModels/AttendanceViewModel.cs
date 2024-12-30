using ClassCheck.Services;
using ClassCheck.Models;
using ClassCheck.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class AttendanceViewModel : BaseStudentViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly MajorViewModel _majorViewModel;
    private readonly AddLessonViewModel _lessonViewModel;
    public ObservableCollection<Major> Majors => _majorViewModel.Majors;
    public ObservableCollection<Lesson> Lessons => _lessonViewModel.Lessons;

    public ObservableCollection<Attendance> Attendances { get; private set; } = [];


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
        set => SetProperty(ref _selectedMajor, value);
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

        _lessonViewModel.LessonAdded += async (sender, lesson) =>
        {
            await LoadMajorsAndLessons();
        };
    }

    private async Task LoadMajorsAndLessons()
    {
        await _majorViewModel.LoadMajorsAsync();
        await _lessonViewModel.LoadLessonsAsync();
    }

    private async Task LoadAttendanceAsync()
    {
        if (SelectedLesson == null || SelectedMajor == null)
        {
            Console.WriteLine("Lesson or Major is not selected!");
            return;
        }

        Console.WriteLine($"Selected Lesson: {SelectedLesson.CourseName}, Selected Major: {SelectedMajor.Name}");

        try
        {
            var attendanceList = await _databaseService.GetAttendanceByFiltersAsync(
                SelectedLesson.Id.ToString(),
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
