using System.Windows.Input;
using ClassCheck.Services;
using ClassCheck.Models;
using ClassCheck.Constants;
using Microsoft.Extensions.Logging;

namespace ClassCheck.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly SecurityService _securityService;
        private readonly UserService _userService;

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
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

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel(DatabaseService databaseService,
                                 SecurityService securityService,
                                 UserService userService,
                                 ILogger<RegisterViewModel> logger) : base(logger)
        {
            _databaseService = databaseService;
            _securityService = securityService;
            _userService = userService;
            RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsBusy);
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync("///addstudent"), () => !IsBusy);
        }

        private async Task RegisterAsync()
        {
            await ExecuteAsync(async () =>
            {
                IsErrorVisible = false;

                if (!ValidateInput())
                    return;

                var existingUser = await _databaseService.GetByEmailAsync(Email);
                if (existingUser != null)
                {
                    ErrorMessage = ValidationMessages.EmailExists;
                    IsErrorVisible = true;
                    return;
                }

                var hashedPassword = _securityService.HashPassword(Password);
                var newUser = new User
                {
                    Username = Username,
                    Email = Email,
                    Password = hashedPassword
                };

                await _databaseService.InsertAsync(newUser);
                _userService.CurrentUser = newUser;
                await Shell.Current.GoToAsync("///home");
            }, "Registration failed");
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = ValidationMessages.RequiredFields;
                IsErrorVisible = true;
                return false;
            }

            if (!IsValidEmail(Email))
            {
                ErrorMessage = ValidationMessages.InvalidEmail;
                IsErrorVisible = true;
                return false;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = ValidationMessages.PasswordTooShort;
                IsErrorVisible = true;
                return false;
            }

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
