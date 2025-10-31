using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;

namespace Desktop.ViewModels;

public partial class TaskDetailViewModel : ObservableObject
{
    private readonly ITaskService _taskService;

    [ObservableProperty]
    private TaskGetDto? _task;

    [ObservableProperty]
    private string _taskName = string.Empty;

    [ObservableProperty]
    private string? _taskDescription;

    [ObservableProperty]
    private DateTime? _dueDate;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    public TaskDetailViewModel(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [RelayCommand]
    private async Task LoadTaskAsync(Guid taskId)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var task = await _taskService.GetByIdAsync(taskId);
            if (task != null)
            {
                Task = task;
                TaskName = task.TaskName;
                TaskDescription = task.TaskDesc;
                DueDate = task.DueDate;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void StartEdit()
    {
        if (Task != null)
        {
            IsEditing = true;
            TaskName = Task.TaskName;
            TaskDescription = Task.TaskDesc;
            DueDate = Task.DueDate;
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        if (Task != null)
        {
            TaskName = Task.TaskName;
            TaskDescription = Task.TaskDesc;
            DueDate = Task.DueDate;
        }
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveTaskAsync()
    {
        if (Task == null || string.IsNullOrWhiteSpace(TaskName))
        {
            ErrorMessage = "Task name cannot be empty";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var taskData = new TaskPutDto { TaskId = Task.TaskId, ProjectId = Task.ProjectId, CategoryId = Task.CategoryId, TaskName = TaskName, TaskDesc = TaskDescription, DueDate = DueDate };
            var updatedTask = await _taskService.UpdateAsync(taskData);
            if (updatedTask != null)
            {
                Task = updatedTask;
                IsEditing = false;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save task: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteTaskAsync()
    {
        if (Task == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _taskService.DeleteAsync(Task.TaskId);
            if (success)
            {
                Task = null;
            }
            else
            {
                ErrorMessage = "Failed to delete task";
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

    public void SetTask(TaskGetDto task)
    {
        Task = task;
        TaskName = task.TaskName;
        TaskDescription = task.TaskDesc;
        DueDate = task.DueDate;
        IsEditing = false;
        ErrorMessage = string.Empty;
    }
}
