using ClassCheck.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class MajorViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private ObservableCollection<Major> _majors;
    private static bool _isLoaded = false;

    public ObservableCollection<Major> Majors
    {
        get => _majors;
        private set
        {
            _majors = value;
            OnPropertyChanged();
        }
    }

    public MajorViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _majors = new ObservableCollection<Major>();

        if (!_isLoaded)
        {
            LoadMajorsAsync().ConfigureAwait(false); // Ensure this is called only once
        }
    }

    public async Task LoadMajorsAsync()
    {
        if (_isLoaded)
            return;

        try
        {
            var majors = await _databaseService.GetAllAsync<Major>(); // Use generic GetAllAsync
            Majors.Clear();
            foreach (var major in majors)
            {
                Majors.Add(major);
                System.Diagnostics.Debug.WriteLine($"Added major: {major.Name}");
            }
            System.Diagnostics.Debug.WriteLine($"Total majors loaded: {Majors.Count}");
            _isLoaded = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading majors: {ex.Message}");
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}