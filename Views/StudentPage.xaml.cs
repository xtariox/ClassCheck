using ClassCheck.ViewModels;
using ClassCheck.Services;

namespace ClassCheck.Views;
public partial class StudentPage : ContentPage
{
    public StudentPage(StudentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        // Trigger the loading of majors and students (this should happen in the constructor of the ViewModel as you have it)
        viewModel.LoadStudentsCommand.Execute(null);
        viewModel.LoadMajorsCommand.Execute(null);
    }
}
