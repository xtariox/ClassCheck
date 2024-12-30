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
    }
}
