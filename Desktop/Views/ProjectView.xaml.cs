using Desktop.ViewModels;
using Entities.DTO;
using System.Diagnostics;

namespace Desktop.Views;

public partial class ProjectView : ContentPage, IQueryAttributable
{
    private readonly BoardViewModel _viewModel;
    private TaskGetDto? _draggedTask;
    private CategoryWithTasksViewModel? _dragOverCategory;

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

    private void OnTaskDragStarting(object? sender, DragStartingEventArgs e)
    {
        TaskGetDto? task = null;
        if (sender is DragGestureRecognizer recognizer && recognizer.Parent is Border border && border.BindingContext is TaskGetDto taskFromBorder)
        {
            task = taskFromBorder;
        }
        else if (sender is BindableObject bindable && bindable.BindingContext is TaskGetDto taskFromContext)
        {
            task = taskFromContext;
        }

        if (task != null)
        {
            _draggedTask = task;
            e.Data.Properties["Task"] = task;
            e.Data.Text = task.TaskId.ToString();
            Debug.WriteLine($"Started dragging task: {task.TaskName} (ID: {task.TaskId}, CategoryId: {task.CategoryId})");
        }
        else
        {
            Debug.WriteLine($"Failed to start drag. Sender: {sender?.GetType().Name}, BindingContext: {(sender as BindableObject)?.BindingContext?.GetType().Name ?? "null"}");
        }
    }

    private void OnCategoryDragOver(object? sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;

        CategoryWithTasksViewModel? category = FindCategoryFromElement(sender);

        if (category != null)
        {
            _dragOverCategory = category;
            Debug.WriteLine($"Drag over category: {category.Category.CategoryName}");
        }
        else
        {
            Debug.WriteLine($"Drag over, couldn't find category. Sender type: {sender?.GetType().Name}, BindingContext: {(sender as BindableObject)?.BindingContext?.GetType().Name ?? "null"}");
        }
    }

    private async void OnCategoryDrop(object? sender, DropEventArgs e)
    {
        Debug.WriteLine($"Drop event triggered. Sender type: {sender?.GetType().Name}");

        CategoryWithTasksViewModel? targetCategory = FindCategoryFromElement(sender);

        if (targetCategory == null && _dragOverCategory != null)
        {
            targetCategory = _dragOverCategory;
            Debug.WriteLine("Using drag over category as target");
        }

        if (targetCategory != null)
        {
            Debug.WriteLine($"Target category: {targetCategory.Category.CategoryName} (ID: {targetCategory.Category.CategoryId})");

            TaskGetDto? task = _draggedTask;

            if (task != null)
            {
                Debug.WriteLine($"Task found from stored reference: {task.TaskName}, Current category: {task.CategoryId}, Target category: {targetCategory.Category.CategoryId}");

                if (task.CategoryId != targetCategory.Category.CategoryId)
                {
                    Debug.WriteLine($"Moving task '{task.TaskName}' to category '{targetCategory.Category.CategoryName}'");
                    try
                    {
                        await _viewModel.MoveTaskToCategoryAsync(task, targetCategory.Category.CategoryId);
                        Debug.WriteLine($"Move completed successfully");
                        await Task.Delay(2000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error moving task: {ex.Message}");
                        await Task.Delay(3000);
                    }
                }
                else
                {
                    Debug.WriteLine("Task already in target category, no move needed");
                    await Task.Delay(2000);
                }
            }
            else
            {
                Debug.WriteLine("Failed to retrieve task - _draggedTask is null");
                await Task.Delay(2000);
            }

            _draggedTask = null;
            _dragOverCategory = null;
        }
        else
        {
            Debug.WriteLine($"Failed to get target category. Sender: {sender?.GetType().Name}");
            await Task.Delay(2000);
            _draggedTask = null;
            _dragOverCategory = null;
        }
    }

    private CategoryWithTasksViewModel? FindCategoryFromElement(object? element)
    {
        if (element == null) return null;

        if (element is BindableObject bindable && bindable.BindingContext is CategoryWithTasksViewModel category)
        {
            Debug.WriteLine($"Found category directly: {category.Category.CategoryName}");
            return category;
        }

        if (element is VisualElement visual)
        {
            var current = visual;
            int depth = 0;
            while (current != null && depth < 10)
            {
                Debug.WriteLine($"  Checking level {depth}: {current.GetType().Name}, BindingContext: {current.BindingContext?.GetType().Name ?? "null"}");

                if (current.BindingContext is CategoryWithTasksViewModel cat)
                {
                    Debug.WriteLine($"  Found category at depth {depth}: {cat.Category.CategoryName}");
                    return cat;
                }
                current = current.Parent as VisualElement;
                depth++;
            }
        }

        return null;
    }
}
