using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class ManageProjectUsersViewModel : ObservableObject
{
    private readonly IProjectService _projectService;
    private readonly Guid _projectId;

    [ObservableProperty]
    private string _projectName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ProjectUserGetDto> _projectUsers = new();

    [ObservableProperty]
    private string _newUserEmail = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    public ManageProjectUsersViewModel(IProjectService projectService, ProjectGetDto project)
    {
        _projectService = projectService;
        _projectId = project.ProjectId;
        ProjectName = project.ProjectName;
        
        foreach (var user in project.ProjectUsers)
        {
            ProjectUsers.Add(user);
        }
    }

    [RelayCommand]
    private async Task AddUserAsync()
    {
        if (string.IsNullOrWhiteSpace(NewUserEmail))
        {
            ErrorMessage = "Please enter a valid email address";
            SuccessMessage = string.Empty;
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            var success = await _projectService.AddUserToProjectAsync(_projectId, NewUserEmail);
            if (success)
            {
                SuccessMessage = "User added successfully!";
                NewUserEmail = string.Empty;
                await RefreshProjectUsersAsync();
            }
            else
            {
                ErrorMessage = "Failed to add user. Please check the email and try again.";
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
    private async Task RemoveUserAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            var success = await _projectService.RemoveUserFromProjectAsync(_projectId, email);
            if (success)
            {
                SuccessMessage = "User removed successfully!";
                await RefreshProjectUsersAsync();
            }
            else
            {
                ErrorMessage = "Failed to remove user.";
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

    private async Task RefreshProjectUsersAsync()
    {
        try
        {
            var projects = await _projectService.GetAllAsync();
            var updatedProject = projects?.FirstOrDefault(p => p.ProjectId == _projectId);
            
            if (updatedProject != null)
            {
                ProjectUsers.Clear();
                foreach (var user in updatedProject.ProjectUsers)
                {
                    ProjectUsers.Add(user);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to refresh users: {ex.Message}";
        }
    }
}
