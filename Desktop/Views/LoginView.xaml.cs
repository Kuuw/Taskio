using Desktop.ViewModels;

namespace Desktop.Views;

public partial class LoginView : ContentPage
{
    public LoginView(AuthViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegisterView));
    }
}
