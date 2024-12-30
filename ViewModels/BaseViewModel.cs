using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ClassCheck.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly ILogger _logger;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
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

        private bool _isSuccessVisible;
        public bool IsSuccessVisible
        {
            get => _isSuccessVisible;
            set => SetProperty(ref _isSuccessVisible, value);
        }

        private string _successMessage;
        public string SuccessMessage
        {
            get => _successMessage;
            set 
            {
                if (SetProperty(ref _successMessage, value))
                {
                    IsSuccessVisible = !string.IsNullOrEmpty(value);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;
            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public BaseViewModel(ILogger logger = null)
        {
            _logger = logger;
        }

        protected async Task<bool> ExecuteAsync(Func<Task> operation, string errorMessage = null)
        {
            try
            {
                IsBusy = true;
                IsErrorVisible = false;
                ErrorMessage = string.Empty;
                await operation?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = errorMessage ?? ex.Message;
                IsErrorVisible = true;
                _logger?.LogError(ex, errorMessage ?? ex.Message);
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected Command CreateCommand(Func<Task> execute)
            => new Command(async () => await ExecuteAsync(execute), () => !IsBusy);
    }
}
