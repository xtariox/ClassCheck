using System.Windows.Input;
using MauiApp1.Models;
using ClassCheck.Services;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace MauiApp1.ViewModels
{
    public class AddStudentViewModel : BaseStudentViewModel
    {
        private readonly DatabaseService _databaseService;

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

        private string _major;
        public string Major
        {
            get => _major;
            set => SetProperty(ref _major, value);
        }

        // Message to display when a student is added
        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        // Commands for Add and Cancel actions
        public ICommand AddStudentCommand { get; }
        public ICommand CancelCommand { get; }




       
        public AddStudentViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Initialize commands
            AddStudentCommand = new Command(async () =>
            {
                System.Diagnostics.Debug.WriteLine("Add button clicked");  // Ajout de log
                await AddStudentAsync();
            });

            CancelCommand = new Command(() =>
            {
                System.Diagnostics.Debug.WriteLine("Cancel button clicked");  // Ajout de log
                Shell.Current.GoToAsync("///home");
            });
        }

        // Method to add a student
        private async Task AddStudentAsync()
        {
            if (ValidateInput())
            {
                // Vérification si un étudiant avec le même numéro de carte ID existe déjà
                var existingStudent = await _databaseService.GetByIDCardNumber(IDCardNumber);
                if (existingStudent != null)
                {
                    SuccessMessage = "A student with this ID card number already exists.";
                    return;
                }

                // Création de l'étudiant
                var newStudent = new Student
                {
                    IDCardNumber = IDCardNumber,
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Major = Major
                };

                // Insertion dans la base de données
                var result = await _databaseService.Insert(newStudent);

                // Mise à jour du message de succès ou d'échec
                if (result > 0)
                {
                    SuccessMessage = "Student added successfully!";
                    ClearFields();  // Clear fields after successful addition
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

        // Simple validation method
        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(IDCardNumber) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(Major);
        }

        // Clear the input fields
        private void ClearFields()
        {
            IDCardNumber = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Major = string.Empty;
        }
    }
}
