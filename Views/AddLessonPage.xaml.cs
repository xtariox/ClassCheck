using ClassCheck;
using ClassCheck.Services;
using ClassCheck.ViewModels;

namespace ClassCheck.Views;

public partial class AddLessonPage : ContentPage
{
    public AddLessonPage()
    {
        InitializeComponent();
        BindingContext = new AddLessonViewModel(
            App.DependencyService.GetRequiredService<DatabaseService>(),
            App.DependencyService.GetRequiredService<MajorViewModel>()
        );
    }
}