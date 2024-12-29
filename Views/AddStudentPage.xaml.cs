using ClassCheck.Services;
using ClassCheck.ViewModels;
using MauiApp1.ViewModels;
using MauiApp1.Views;
using MauiApp1.Views;

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