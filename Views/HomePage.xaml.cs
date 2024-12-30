using ClassCheck.ViewModels;
using System.Windows.Input;

namespace ClassCheck.Views;

public partial class HomePage : ContentPage
{

   
    public HomePage()
    {
        InitializeComponent();
        InitializeComponent();
        BindingContext = new HomePageViewModel();
    }


}




