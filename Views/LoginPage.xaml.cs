using ClassCheck.Services;
using ClassCheck.ViewModels;

namespace ClassCheck.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly DatabaseService _dbService;

        public LoginPage(LoginViewModel viewModel, DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            BindingContext = viewModel;
        }
    }
}
