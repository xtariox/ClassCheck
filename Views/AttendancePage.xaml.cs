using ClassCheck;
using ClassCheck.Services;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class AttendancePage : ContentPage
{
    public AttendancePage()
    {
        InitializeComponent();
        BindingContext = new AttendanceViewModel(
            App.DependencyService.GetRequiredService<DatabaseService>(),
            App.DependencyService.GetRequiredService<MajorViewModel>(),
            App.DependencyService.GetRequiredService<AddLessonViewModel>()
        );

    }
}
