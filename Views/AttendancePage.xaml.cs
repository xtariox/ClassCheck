using ClassCheck;
using ClassCheck.Services;
using ClassCheck.ViewModels;

namespace ClassCheck.Views;

public partial class AttendancePage : ContentPage
{
    public AttendancePage()
    {
        InitializeComponent();
        BindingContext = App.DependencyService.GetRequiredService<AttendanceViewModel>();
        Appearing += AttendancePage_Appearing;
    }

    private async void AttendancePage_Appearing(object sender, EventArgs e)
    {
        var viewModel = BindingContext as AttendanceViewModel;
        if (viewModel != null)
        {
            await viewModel.LoadMajorsAndLessons();
            viewModel.FilterLessons(); // Ensure lessons are filtered after loading
        }
    }
}
