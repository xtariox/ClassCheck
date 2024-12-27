using MauiApp1.ViewModels;

namespace MauiApp1.Views;

public partial class AddLessonPage : ContentPage
{
	public AddLessonPage(AddLessonViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }

   
}