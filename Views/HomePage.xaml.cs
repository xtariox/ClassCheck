using MauiApp1.ViewModels;
using System.Windows.Input;

namespace MauiApp1.Views;

public partial class HomePage : ContentPage
{

   
    public HomePage()
    {
        InitializeComponent();
        InitializeComponent();
        BindingContext = new HomePageViewModel();
    }


}




