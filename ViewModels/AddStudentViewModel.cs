using System.Windows.Input;
using MauiApp1.Models;
using System.Collections.ObjectModel;
using ClassCheck.Services;
using System.Diagnostics;

namespace MauiApp1.ViewModels
{
    public class AddStudentViewModel : BaseStudentViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly MajorViewModel _majorViewModel;

        public ObservableCollection<Major> Majors => _majorViewModel.Majors;

        // Properties to bind to UI
        private string _idcardnumber;
        public string IDCardNumber
        {
            get => _idcardnumber;
            set => SetProperty(ref _idcardnumber, value);
        }

        private string _firstname;
        public string FirstName
        {
            get => _firstname;
            set => SetProperty(ref _firstname, value);
        }

        private string _lastname;
        public string LastName
        {
            get => _lastname;
            set => SetProperty(ref _lastname, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _phonenumber;
        public string PhoneNumber
        {
            get => _phonenumber;
            set => SetProperty(ref _phonenumber, value);
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

        public ICommand AddStudentCommand { get; }
        public ICommand CancelCommand { get; }

        public AddStudentViewModel(DatabaseService databaseService, MajorViewModel majorViewModel)
        {
            _databaseService = databaseService;
            _majorViewModel = majorViewModel;

            // Load majors
            LoadMajorsAsync().ConfigureAwait(false);

            // Initialize commands
            AddStudentCommand = new Command(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Add button clicked");
                await AddStudentAsync();
            });

            CancelCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("Cancel button clicked");
                Shell.Current.GoToAsync("///home");
            });
        }

        // Method to load majors asynchronously (ensuring it works)
        public async Task LoadMajorsAsync()
        {
            try
            {
                await _majorViewModel.LoadMajorsAsync(); // Ensure majors are loaded
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading majors: {ex.Message}");
            }
        }

        private async Task AddStudentAsync()
        {
            try
            {
                if (ValidateInput())
                {
                    var existingStudent = await _databaseService.GetByIDCardNumber(IDCardNumber);
                    if (existingStudent != null)
                    {
                        SuccessMessage = "A student with this ID card number already exists.";
                        return;
                    }

                    if (Major == null)
                    {
                        SuccessMessage = "Please select a valid major.";
                        return;
                    }

                    var newStudent = new Student
                    {
                        IDCardNumber = IDCardNumber,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        Major = Major.Name // Store the Major ID
                    };

                    var result = await _databaseService.Insert(newStudent);

                    if (result > 0)
                    {
                        SuccessMessage = "Student added successfully!";
                        ClearFields();
                    }
                    else
                    {
                        SuccessMessage = "Failed to add the student.";
                    }
                }
                else
                {
                    SuccessMessage = "Please fill in all the required fields.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in AddStudentAsync: {ex.Message}");
                SuccessMessage = "An error occurred while adding the student.";
            }
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(IDCardNumber) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(Major?.Name);
        }

        private void ClearFields()
        {
            IDCardNumber = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Major.Name = string.Empty; // Clear Major selection
        }
    }
}
