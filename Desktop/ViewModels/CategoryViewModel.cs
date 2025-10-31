using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class CategoryViewModel : ObservableObject
{
    private readonly ICategoryService _categoryService;
    private readonly ITaskService _taskService;

    [ObservableProperty]
    private CategoryGetDto? _category;

    [ObservableProperty]
    private ObservableCollection<TaskGetDto> _tasks = new();

    [ObservableProperty]
    private string _newTaskName = string.Empty;

    [ObservableProperty]
    private string? _newTaskDescription;

    [ObservableProperty]
    private DateTime? _newTaskDueDate;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CategoryViewModel(ICategoryService categoryService, ITaskService taskService)
    {
        _categoryService = categoryService;
        _taskService = taskService;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoadCategoryAsync(Guid categoryId)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var category = await _categoryService.GetByIdAsync(categoryId);
            if (category != null)
            {
                Category = category;
                Tasks.Clear();
                foreach (var taskItem in category.Tasks)
                {
                    Tasks.Add(taskItem);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load category: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task UpdateCategoryAsync()
    {
        if (Category == null || string.IsNullOrWhiteSpace(Category.CategoryName))
        {
            ErrorMessage = "Invalid category data";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var categoryData = new CategoryPutDto { CategoryId = Category.CategoryId, ProjectId = Category.ProjectId, CategoryName = Category.CategoryName, SortOrder = Category.SortOrder };
            var updatedCategory = await _categoryService.UpdateAsync(categoryData);
            if (updatedCategory != null)
            {
                Category = updatedCategory;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update category: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task CreateTaskAsync()
    {
        if (Category == null || string.IsNullOrWhiteSpace(NewTaskName))
        {
            ErrorMessage = "Task name cannot be empty";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var taskData = new TaskPostDto { ProjectId = Category.ProjectId, CategoryId = Category.CategoryId, TaskName = NewTaskName, TaskDesc = NewTaskDescription, DueDate = NewTaskDueDate };
            var newTask = await _taskService.CreateAsync(taskData);
            if (newTask != null)
            {
                Tasks.Add(newTask);
                NewTaskName = string.Empty;
                NewTaskDescription = null;
                NewTaskDueDate = null;
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

    [RelayCommand]
    private async System.Threading.Tasks.Task DeleteTaskAsync(TaskGetDto? task)
    {
        if (task == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _taskService.DeleteAsync(task.TaskId);
            if (success)
            {
                Tasks.Remove(task);
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

    [RelayCommand]
    private async System.Threading.Tasks.Task UpdateTaskAsync(TaskGetDto task)
    {
        if (task == null || string.IsNullOrWhiteSpace(task.TaskName))
        {
            ErrorMessage = "Invalid task data";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var taskData = new TaskPutDto { TaskId = task.TaskId, ProjectId = task.ProjectId, CategoryId = task.CategoryId, TaskName = task.TaskName, TaskDesc = task.TaskDesc, DueDate = task.DueDate };
            var updatedTask = await _taskService.UpdateAsync(taskData);
            if (updatedTask != null)
            {
                var index = Tasks.IndexOf(task);
                if (index >= 0)
                {
                    Tasks[index] = updatedTask;
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
