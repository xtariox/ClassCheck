using ClassCheck.Services;
using ClassCheck.ViewModels;
using ClassCheck.ViewModels;
using ClassCheck.Views;
using ClassCheck.Views;

namespace ClassCheck.Views;

public partial class AddStudentPage : ContentPage
{
    public AddStudentPage()
    {
        InitializeComponent();
        BindingContext = new AddStudentViewModel(
            App.DependencyService.GetRequiredService<DatabaseService>(),
            App.DependencyService.GetRequiredService<MajorViewModel>()
        );
    }
}