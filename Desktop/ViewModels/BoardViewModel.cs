using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class BoardViewModel : ObservableObject
{
    private readonly IProjectService _projectService;
    private readonly ICategoryService _categoryService;
    private readonly ITaskService _taskService;

    [ObservableProperty]
    private ProjectGetDto? _project;

    [ObservableProperty]
    private ObservableCollection<CategoryWithTasksViewModel> _categoryColumns = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public BoardViewModel(IProjectService projectService, ICategoryService categoryService, ITaskService taskService)
    {
        _projectService = projectService;
        _categoryService = categoryService;
        _taskService = taskService;
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterTasks();
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoadBoardAsync(Guid projectId)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var projects = await _projectService.GetAllAsync();
            if (projects != null)
            {
                var project = projects.FirstOrDefault(p => p.ProjectId == projectId);
                if (project != null)
                {
                    Project = project;
                    await LoadCategoriesAndTasksAsync();
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load board: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async System.Threading.Tasks.Task LoadCategoriesAndTasksAsync()
    {
        if (Project == null) return;
        CategoryColumns.Clear();
        var sortedCategories = Project.Categories.OrderBy(c => c.SortOrder).ToList();
        foreach (var category in sortedCategories)
        {
            var categoryViewModel = new CategoryWithTasksViewModel(this) { Category = category };
            var sortedTasks = category.Tasks.OrderBy(t => t.SortOrder).ToList();
            foreach (var taskItem in sortedTasks)
            {
                categoryViewModel.Tasks.Add(taskItem);
                categoryViewModel.FilteredTasks.Add(taskItem);
            }
            CategoryColumns.Add(categoryViewModel);
        }
        await System.Threading.Tasks.Task.CompletedTask;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task RefreshBoardAsync()
    {
        if (Project != null)
        {
            await LoadBoardAsync(Project.ProjectId);
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task CreateCategoryAsync(string categoryName)
    {
        if (Project == null || string.IsNullOrWhiteSpace(categoryName))
        {
            ErrorMessage = "Category name cannot be empty";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var categoryData = new CategoryPostDto { ProjectId = Project.ProjectId, CategoryName = categoryName, SortOrder = CategoryColumns.Count };
            var newCategory = await _categoryService.CreateAsync(categoryData);
            if (newCategory != null)
            {
                var categoryViewModel = new CategoryWithTasksViewModel(this) { Category = newCategory };
                CategoryColumns.Add(categoryViewModel);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create category: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task DeleteCategoryAsync(CategoryWithTasksViewModel? categoryViewModel)
    {
        if (categoryViewModel?.Category == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _categoryService.DeleteAsync(categoryViewModel.Category.CategoryId);
            if (success)
            {
                CategoryColumns.Remove(categoryViewModel);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete category: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async System.Threading.Tasks.Task CreateTaskInCategoryAsync(CategoryWithTasksViewModel categoryViewModel, string taskName)
    {
        if (Project == null || categoryViewModel?.Category == null || string.IsNullOrWhiteSpace(taskName))
        {
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var taskData = new TaskPostDto { ProjectId = Project.ProjectId, CategoryId = categoryViewModel.Category.CategoryId, TaskName = taskName, TaskDesc = null, DueDate = null };
            var newTask = await _taskService.CreateAsync(taskData);
            if (newTask != null)
            {
                categoryViewModel.Tasks.Add(newTask);
                categoryViewModel.FilteredTasks.Add(newTask);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async System.Threading.Tasks.Task MoveTaskToCategoryAsync(TaskGetDto taskItem, Guid targetCategoryId)
    {
        if (taskItem == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var oldCategory = CategoryColumns.FirstOrDefault(c => c.Category.CategoryId == taskItem.CategoryId);
            var taskData = new TaskPutDto { TaskId = taskItem.TaskId, ProjectId = taskItem.ProjectId, CategoryId = targetCategoryId, TaskName = taskItem.TaskName, TaskDesc = taskItem.TaskDesc, DueDate = taskItem.DueDate };
            var updatedTask = await _taskService.UpdateAsync(taskData);
            if (updatedTask != null)
            {
                if (oldCategory != null)
                {
                    var taskToRemove = oldCategory.Tasks.FirstOrDefault(t => t.TaskId == taskItem.TaskId);
                    if (taskToRemove != null)
                    {
                        oldCategory.Tasks.Remove(taskToRemove);
                        oldCategory.FilteredTasks.Remove(taskToRemove);
                    }
                }
                var newCategory = CategoryColumns.FirstOrDefault(c => c.Category.CategoryId == targetCategoryId);
                if (newCategory != null)
                {
                    newCategory.Tasks.Add(updatedTask);
                    newCategory.FilteredTasks.Add(updatedTask);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to move task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void FilterTasks()
    {
        foreach (var categoryColumn in CategoryColumns)
        {
            categoryColumn.FilterTasks(SearchText);
        }
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(TaskGetDto taskItem)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _taskService.DeleteAsync(taskItem.TaskId);
            if (success)
            {
                var category = CategoryColumns.FirstOrDefault(c => c.Category.CategoryId == taskItem.CategoryId);
                if (category != null)
                {
                    var taskToRemove = category.Tasks.FirstOrDefault(t => t.TaskId == taskItem.TaskId);
                    if (taskToRemove != null)
                    {
                        category.Tasks.Remove(taskToRemove);
                        category.FilteredTasks.Remove(taskToRemove);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(TaskGetDto taskItem)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var taskData = new TaskPutDto { TaskId = taskItem.TaskId, ProjectId = taskItem.ProjectId, CategoryId = taskItem.CategoryId, TaskName = taskItem.TaskName, TaskDesc = taskItem.TaskDesc, DueDate = taskItem.DueDate };
            var updatedTask = await _taskService.UpdateAsync(taskData);
            if (updatedTask != null)
            {
                var category = CategoryColumns.FirstOrDefault(c => c.Category.CategoryId == taskItem.CategoryId);
                if (category != null)
                {
                    var index = category.Tasks.IndexOf(taskItem);
                    if (index >= 0)
                    {
                        category.Tasks[index] = updatedTask;
                    }
                    var filteredIndex = category.FilteredTasks.IndexOf(taskItem);
                    if (filteredIndex >= 0)
                    {
                        category.FilteredTasks[filteredIndex] = updatedTask;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

public partial class CategoryWithTasksViewModel : ObservableObject
{
    private readonly BoardViewModel _boardViewModel;

    [ObservableProperty]
    private CategoryGetDto _category = null!;

    [ObservableProperty]
    private ObservableCollection<TaskGetDto> _tasks = new();

    [ObservableProperty]
    private ObservableCollection<TaskGetDto> _filteredTasks = new();

    [ObservableProperty]
    private string _newTaskName = string.Empty;

    [ObservableProperty]
    private bool _isAddingTask;

    public CategoryWithTasksViewModel(BoardViewModel boardViewModel)
    {
        _boardViewModel = boardViewModel;
    }

    [RelayCommand]
    private void StartAddingTask()
    {
        IsAddingTask = true;
    }

    [RelayCommand]
    private void CancelAddingTask()
    {
        IsAddingTask = false;
        NewTaskName = string.Empty;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task AddTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskName))
        {
            return;
        }
        await _boardViewModel.CreateTaskInCategoryAsync(this, NewTaskName);
        NewTaskName = string.Empty;
        IsAddingTask = false;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task DeleteTaskAsync(TaskGetDto? taskItem)
    {
        if (taskItem == null) return;
        await _boardViewModel.DeleteTaskAsync(taskItem);
    }

    public void FilterTasks(string searchText)
    {
        FilteredTasks.Clear();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            foreach (var taskItem in Tasks)
            {
                FilteredTasks.Add(taskItem);
            }
        }
        else
        {
            var searchLower = searchText.ToLower();
            foreach (var taskItem in Tasks)
            {
                if (taskItem.TaskName.ToLower().Contains(searchLower) || (taskItem.TaskDesc?.ToLower().Contains(searchLower) ?? false))
                {
                    FilteredTasks.Add(taskItem);
                }
            }
        }
    }
}
