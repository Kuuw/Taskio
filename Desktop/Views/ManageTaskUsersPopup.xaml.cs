using Desktop.ViewModels;

namespace Desktop.Views;

public partial class ManageTaskUsersPopup : ContentPage
{
    public ManageTaskUsersPopup(ManageTaskUsersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
