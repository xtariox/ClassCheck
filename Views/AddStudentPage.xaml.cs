using ClassCheck.Services;
using ClassCheck.ViewModels;
using Microsoft.Extensions.Logging;

namespace ClassCheck.Views;

public partial class AddStudentPage : ContentPage
{
    public AddStudentPage()
    {
        InitializeComponent();
        BindingContext = new AddStudentViewModel(
            App.DependencyService.GetRequiredService<DatabaseService>(),
            App.DependencyService.GetRequiredService<MajorViewModel>(),
            App.DependencyService.GetRequiredService<ILogger<AddStudentViewModel>>()
        );
    }
}