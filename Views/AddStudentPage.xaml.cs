using ClassCheck.ViewModels;

using MauiApp1.ViewModels;


namespace ClassCheck.Views;

public partial class AddStudentPage : ContentPage
{
    public AddStudentPage(AddStudentViewModel viewModelSt)
    {
        InitializeComponent();
        BindingContext=viewModelSt;
        
    }




}