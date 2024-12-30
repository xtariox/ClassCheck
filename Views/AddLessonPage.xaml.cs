using ClassCheck.Services;
using ClassCheck.ViewModels;
using Microsoft.Extensions.Logging;

namespace ClassCheck.Views;

public partial class AddLessonPage : ContentPage
{
    public AddLessonPage()
    {
        InitializeComponent();
        BindingContext = App.DependencyService.GetRequiredService<AddLessonViewModel>();
    }
}