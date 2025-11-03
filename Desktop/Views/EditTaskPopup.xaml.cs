using Desktop.ViewModels;

namespace Desktop.Views;

public partial class EditTaskPopup : ContentPage
{
    public EditTaskPopup(EditTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
