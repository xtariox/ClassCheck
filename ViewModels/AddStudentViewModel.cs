using ClassCheck.Models;
using ClassCheck.Services;
using ClassCheck.Constants;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace ClassCheck.ViewModels
{
    public class AddStudentViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly MajorViewModel _majorViewModel;

        public ObservableCollection<Major> Majors => _majorViewModel.Majors;

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

        public AddStudentViewModel(
            DatabaseService databaseService, 
            MajorViewModel majorViewModel,
            ILogger<AddStudentViewModel> logger) : base(logger)
        {
            _databaseService = databaseService;
            _majorViewModel = majorViewModel;


            // Initialize commands
            AddStudentCommand = new Command(async () => await AddStudentAsync());
            CancelCommand = new Command(() => Shell.Current.GoToAsync("///home"));

            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await ExecuteAsync(async () =>
            {
                await _majorViewModel.LoadMajorsAsync();
            }, "Failed to load majors");
        }

        private async Task AddStudentAsync()
        {
            await ExecuteAsync(async () =>
            {
                // Clear previous messages
                SuccessMessage = string.Empty;
                ErrorMessage = string.Empty;

                if (!ValidateInput())
                {
                    ErrorMessage = ValidationMessages.RequiredFields;
                    IsErrorVisible = true;
                    return;
                }

                var existingStudent = await _databaseService.GetByIDCardNumberAsync(IDCardNumber);
                if (existingStudent != null)
                {
                    ErrorMessage = ValidationMessages.StudentIDExists;
                    IsErrorVisible = true;
                    return;
                }

                var newStudent = new Student
                {
                    IDCardNumber = IDCardNumber,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Major = Major.Name
                };

                var result = await _databaseService.InsertAsync(newStudent);
                if (result <= 0)
                {
                    ErrorMessage = "Failed to add the student to database";
                    IsErrorVisible = true;
                    _logger?.LogError("InsertAsync returned non-positive result for new student.");
                    return;
                }

                SuccessMessage = "Student added successfully!";
                IsSuccessVisible = true;
                ClearFields();

            }, "Failed to add student");
        }

        private bool ValidateInput() =>
            !string.IsNullOrWhiteSpace(IDCardNumber) &&
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(Email) &&
            !string.IsNullOrWhiteSpace(PhoneNumber) &&
            Major != null;

        private void ClearFields()
        {
            IDCardNumber = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Major = null;
        }
    }
}
