using ClassCheck.Constants;
using ClassCheck.Models;
using ClassCheck.Services;
using ClassCheck.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace ClassCheck.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly Services.UserService _userService;

        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ICommand LogoutCommand { get; }

        public MainPageViewModel(DatabaseService databaseService,
                                 Services.UserService userService,
                                 ILogger<MainPageViewModel> logger) : base(logger)
        {
            _databaseService = databaseService;
            _userService = userService;
            LogoutCommand = new Command(async () => await LogoutAsync(), () => !IsBusy);
            Initialize();
        }

        private void Initialize()
        {
            CurrentUser = _userService.CurrentUser;
            if (CurrentUser != null)
            {
                WelcomeMessage = $"Welcome, {CurrentUser.Username}!";
            }
            else
            {
                WelcomeMessage = "Welcome!";
            }
        }

        private async Task LogoutAsync()
        {
            await Shell.Current.GoToAsync("///login");
        }
    }
}
