using Desktop.ViewModels;

namespace Desktop.Views;

public partial class RegisterView : ContentPage
{
    public RegisterView(AuthViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginView");
    }
}
