using ClassCheck.Services;
using ClassCheck.Views;
using System.Windows.Input;
using ClassCheck.Constants;
using Microsoft.Extensions.Logging;

namespace ClassCheck.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly SecurityService _securityService;
        private readonly UserService _userService;

        // Properties bound to the UI
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

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(DatabaseService databaseService,
                              SecurityService securityService,
                              UserService userService,
                              ILogger<LoginViewModel> logger) : base(logger)
        {
            _databaseService = databaseService;
            _securityService = securityService;
            _userService = userService;
            LoginCommand = new Command(async () => await OnLoginClicked(), () => !IsBusy);
            RegisterCommand = new Command(async () => await OnRegisterClicked(), () => !IsBusy);
        }

        // Logic for login button click
        private async Task OnLoginClicked()
        {
            await ExecuteAsync(async () =>
            {
                IsErrorVisible = false;

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = ValidationMessages.EmptyCredentials;
                    IsErrorVisible = true;
                    return;
                }

                var user = await _databaseService.GetByEmailAsync(Email);

                if (user != null && _securityService.VerifyPassword(Password, user.Password))
                {
                    _userService.CurrentUser = user;
                    // Navigate to MainPage after successful login
                    await Shell.Current.GoToAsync("///home");
                }
                else
                {
                    // Show error if login fails
                    ErrorMessage = ValidationMessages.InvalidCredentials;
                    IsErrorVisible = true;
                }
            }, "Login failed");
        }

        // Logic for register button click (navigate to RegisterPage)
        private static async Task OnRegisterClicked()
        {
            await Shell.Current.GoToAsync("///register");
        }
    }
}