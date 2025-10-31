using Desktop.ViewModels;

namespace Desktop.Views;

public partial class ProjectListView : ContentPage
{
    public ProjectListView(ProjectListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProjectListViewModel viewModel)
        {
            viewModel.LoadProjectsCommand.Execute(null);
        }
    }
}
