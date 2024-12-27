using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
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

        // Implement the INotifyPropertyChanged interface to notify the View of changes
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to trigger PropertyChanged for binding updates
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // A common method for updating properties (used in LoginViewModel and RegisterViewModel)
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;
            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public BaseViewModel(ILogger logger)
        {
            _logger = logger;
        }

        protected virtual async Task NavigateAsync()
        {
            // Navigation logic, can be overridden in child ViewModels if needed
        }

        protected async Task ExecuteAsync(Func<Task> operation, string errorMessage = null)
        {
            try
            {
                IsBusy = true;
                await operation?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage ?? ex.Message);
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
