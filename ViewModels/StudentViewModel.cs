using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ClassCheck.Services;
using ClassCheck.Models;
using Microsoft.Extensions.Logging;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.DataGrid;

namespace ClassCheck.ViewModels
{
    public class StudentViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        // Observable collections
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<Student> FilteredStudents { get; set; }

        public ICommand CancelCommand { get; }

        // Selected major
        private string _selectedMajor;
        public string SelectedMajor
        {
            get => _selectedMajor;
            set
            {
                if (_selectedMajor != value)
                {
                    _selectedMajor = value;
                    OnPropertyChanged(nameof(SelectedMajor));
                    FilterStudents();
                }
            }
        }

        private ObservableCollection<string> _majors;
        public ObservableCollection<string> Majors
        {
            get => _majors;
            set
            {
                _majors = value;
                OnPropertyChanged(nameof(Majors));
            }
        }

        // Search settings
        private DataGridSearchPanelVisibilityMode _searchPanelVisibilityMode;
        public DataGridSearchPanelVisibilityMode SearchPanelVisibilityMode
        {
            get => _searchPanelVisibilityMode;
            set
            {
                _searchPanelVisibilityMode = value;
                OnPropertyChanged(nameof(SearchPanelVisibilityMode));
            }
        }

        private DataGridSearchTrigger _searchTrigger;
        public DataGridSearchTrigger SearchTrigger
        {
            get => _searchTrigger;
            set
            {
                _searchTrigger = value;
                OnPropertyChanged(nameof(SearchTrigger));
            }
        }

        public Command LoadStudentsCommand { get; private set; }
        public Command LoadMajorsCommand { get; private set; }

        public StudentViewModel(DatabaseService databaseService, ILogger logger = null) : base(logger)
        {
            _databaseService = databaseService;
            Students = new ObservableCollection<Student>();
            FilteredStudents = new ObservableCollection<Student>();
            Majors = new ObservableCollection<string>();

            // Initialize search settings
            SearchPanelVisibilityMode = DataGridSearchPanelVisibilityMode.AlwaysVisible;
            SearchTrigger = DataGridSearchTrigger.TextChanged;

            CancelCommand = new Command(() =>
            {
                ClearFields();
                Shell.Current.GoToAsync("///home");
            });

            // Execute commands to load data
            LoadStudentsCommand = new Command(async () => await LoadStudentsAsync(), () => !IsBusy);
            LoadStudentsCommand.Execute(null);

            LoadMajorsCommand = new Command(async () => await LoadMajorsAsync(), () => !IsBusy);
            LoadMajorsCommand.Execute(null);
        }

        private async Task LoadStudentsAsync()
        {
            Console.WriteLine("Loading students...");
            bool success = await ExecuteAsync(async () =>
            {
                var students = await _databaseService.GetAllAsync<Student>();
                Students.Clear();
                FilteredStudents.Clear();

                foreach (var student in students)
                {
                    Students.Add(student);
                    FilteredStudents.Add(student);

                    if (!Majors.Contains(student.Major))
                    {
                        Majors.Add(student.Major);
                    }
                }
                OnPropertyChanged(nameof(Students));
            }, "Failed to load students.");

            if (success)
            {
                SuccessMessage = "Students loaded successfully!";
                Console.WriteLine("Students loaded successfully!");
            }
            else
            {
                ErrorMessage = "Error occurred while loading students.";
                Console.WriteLine("Error occurred while loading students.");
            }
        }

        private async Task LoadMajorsAsync()
        {
            Console.WriteLine("Loading majors...");
            var majors = await _databaseService.GetMajorsAsync();
            Majors.Clear();
            foreach (var major in majors)
            {
                Majors.Add(major.Name);
            }
            OnPropertyChanged(nameof(Majors));

            Console.WriteLine($"Total majors loaded: {Majors.Count}");
        }

        private void FilterStudents()
        {
            FilteredStudents.Clear();

            if (string.IsNullOrEmpty(SelectedMajor))
            {
                foreach (var student in Students)
                {
                    FilteredStudents.Add(student);
                }
            }
            else
            {
                foreach (var student in Students.Where(s => s.Major == SelectedMajor))
                {
                    FilteredStudents.Add(student);
                }
            }
        }

        private void ClearFields()
        {
            SelectedMajor = null;
            FilteredStudents.Clear();
            foreach (var student in Students)
            {
                FilteredStudents.Add(student);
            }
            OnPropertyChanged(nameof(FilteredStudents));
        }
    }
}
