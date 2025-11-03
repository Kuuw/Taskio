using Desktop.ViewModels;

namespace Desktop.Views;

public partial class ManageProjectUsersPopup : ContentPage
{
    public ManageProjectUsersPopup(ManageProjectUsersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
