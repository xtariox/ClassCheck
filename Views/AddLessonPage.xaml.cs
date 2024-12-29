using ClassCheck;
using ClassCheck.Services;
using MauiApp1.ViewModels;

namespace MauiApp1.Views;

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