using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;

namespace Desktop.ViewModels;

public partial class EditTaskViewModel : ObservableObject
{
    private readonly ITaskService _taskService;
    private readonly Guid _taskId;
    private readonly Guid _projectId;
    private readonly Guid _categoryId;
    private readonly Action _onTaskUpdated;

    [ObservableProperty]
    private string _taskName = string.Empty;

    [ObservableProperty]
    private string? _taskDescription;

    [ObservableProperty]
    private DateTime _dueDate = DateTime.Today;

    [ObservableProperty]
    private bool _hasDueDate;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public EditTaskViewModel(ITaskService taskService, TaskGetDto task, Action onTaskUpdated)
    {
        _taskService = taskService;
        _taskId = task.TaskId;
        _projectId = task.ProjectId;
        _categoryId = task.CategoryId;
        _onTaskUpdated = onTaskUpdated;

        TaskName = task.TaskName;
        TaskDescription = task.TaskDesc;
        
        if (task.DueDate.HasValue)
        {
            DueDate = task.DueDate.Value;
            HasDueDate = true;
        }
        else
        {
            DueDate = DateTime.Today;
            HasDueDate = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(TaskName))
        {
            ErrorMessage = "Task name is required";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var taskData = new TaskPutDto
            {
                TaskId = _taskId,
                ProjectId = _projectId,
                CategoryId = _categoryId,
                TaskName = TaskName.Trim(),
                TaskDesc = string.IsNullOrWhiteSpace(TaskDescription) ? null : TaskDescription.Trim(),
                DueDate = HasDueDate ? DueDate : null
            };

            var updatedTask = await _taskService.UpdateAsync(taskData);
            
            if (updatedTask != null)
            {
                _onTaskUpdated?.Invoke();
                await Shell.Current.Navigation.PopModalAsync();
            }
            else
            {
                ErrorMessage = "Failed to update task";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }
}
