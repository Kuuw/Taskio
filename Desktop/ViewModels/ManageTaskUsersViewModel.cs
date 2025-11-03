using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class ManageTaskUsersViewModel : ObservableObject
{
    private readonly ITaskService _taskService;
    private readonly Guid _taskId;

    [ObservableProperty]
    private string _taskName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ProjectUserGetDto> _projectUsers = new();

    [ObservableProperty]
    private ObservableCollection<UserGetDto> _assignedUsers = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public ManageTaskUsersViewModel(ITaskService taskService, TaskGetDto task, List<ProjectUserGetDto> projectUsers)
    {
        _taskService = taskService;
        _taskId = task.TaskId;
        TaskName = task.TaskName;
        
        foreach (var user in projectUsers)
        {
            ProjectUsers.Add(user);
        }

        foreach (var user in task.Users)
        {
            AssignedUsers.Add(user);
        }
    }

    [RelayCommand]
    private async Task ToggleUserAssignmentAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        var isAssigned = AssignedUsers.Any(u => u.Email == email);

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            bool success;
            if (isAssigned)
            {
                success = await _taskService.RemoveUserAsync(_taskId, email);
                if (success)
                {
                    var userToRemove = AssignedUsers.FirstOrDefault(u => u.Email == email);
                    if (userToRemove != null)
                    {
                        AssignedUsers.Remove(userToRemove);
                    }
                    SuccessMessage = "User removed from task successfully!";
                }
                else
                {
                    ErrorMessage = "Failed to remove user from task.";
                }
            }
            else
            {
                success = await _taskService.AddUserAsync(_taskId, email);
                if (success)
                {
                    await RefreshAssignedUsersAsync();
                    SuccessMessage = "User assigned to task successfully!";
                }
                else
                {
                    ErrorMessage = "Failed to assign user to task.";
                }
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
    private async Task CloseAsync()
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

    private async Task RefreshAssignedUsersAsync()
    {
        try
        {
            var updatedTask = await _taskService.GetByIdAsync(_taskId);
            
            if (updatedTask != null)
            {
                AssignedUsers.Clear();
                foreach (var user in updatedTask.Users)
                {
                    AssignedUsers.Add(user);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to refresh assigned users: {ex.Message}";
        }
    }

    public bool IsUserAssigned(string email)
    {
        return AssignedUsers.Any(u => u.Email == email);
    }
}
