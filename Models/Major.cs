using ClassCheck.Models;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Major : INotifyPropertyChanged, IEntity
{
    private int _id;
    private string _name;

    [PrimaryKey, AutoIncrement]
    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    [NotNull]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
