using Desktop.ViewModels;
using Entities.DTO;

namespace Desktop.Views;

public partial class ProjectView : ContentPage, IQueryAttributable
{
    private readonly BoardViewModel _viewModel;

    public ProjectView(BoardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Project", out var projectObj) && projectObj is ProjectGetDto project)
        {
            _viewModel.Project = project;
            _viewModel.LoadBoardCommand.Execute(project.ProjectId);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    private void OnAddCategoryTapped(object sender, EventArgs e)
    {
        AddCategoryBorder.IsVisible = true;
        NewCategoryEntry.Focus();
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        if (_viewModel != null && !string.IsNullOrWhiteSpace(NewCategoryEntry.Text))
        {
            await _viewModel.CreateCategoryCommand.ExecuteAsync(NewCategoryEntry.Text);
            NewCategoryEntry.Text = string.Empty;
            AddCategoryBorder.IsVisible = false;
        }
    }

    private void OnCancelAddCategoryClicked(object sender, EventArgs e)
    {
        NewCategoryEntry.Text = string.Empty;
        AddCategoryBorder.IsVisible = false;
    }

    private async void OnDeleteCategoryClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is CategoryWithTasksViewModel categoryViewModel)
        {
            var categoryName = categoryViewModel.Category?.CategoryName ?? "this category";
            var taskCount = categoryViewModel.Tasks?.Count ?? 0;

            var message = taskCount > 0
        ? $"Are you sure you want to delete '{categoryName}'? This will also delete {taskCount} task(s)."
   : $"Are you sure you want to delete '{categoryName}'?";

            bool answer = await DisplayAlert(
               "Delete Category",
                       message,
                  "Delete",
                 "Cancel");

            if (answer && _viewModel != null)
            {
                await _viewModel.DeleteCategoryCommand.ExecuteAsync(categoryViewModel);
            }
        }
    }
}
