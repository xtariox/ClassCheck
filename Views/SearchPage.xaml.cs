using ClassCheck.ViewModels;

namespace ClassCheck.Views;
public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
    }

    public SearchPage(SearchViewModel vm)
    {
		InitializeComponent();
        BindingContext = vm;
    }
}