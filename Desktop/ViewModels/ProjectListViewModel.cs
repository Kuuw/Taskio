using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Desktop.Views;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class ProjectListViewModel : ObservableObject
{
    private readonly IProjectService _projectService;

    [ObservableProperty]
    private ObservableCollection<ProjectGetDto> _projects = new();

    [ObservableProperty]
    private ProjectGetDto? _selectedProject;

    [ObservableProperty]
    private string _newProjectName = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public AuthViewModel AuthViewModel { get; }

    public ProjectListViewModel(IProjectService projectService, AuthViewModel authViewModel)
    {
        _projectService = projectService;
        AuthViewModel = authViewModel;
    }

    [RelayCommand]
    private async Task LoadProjectsAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var projects = await _projectService.GetAllAsync();
            if (projects != null)
            {
                Projects.Clear();
                foreach (var project in projects)
                {
                    Projects.Add(project);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load projects: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreateProjectAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProjectName))
        {
            ErrorMessage = "Project name cannot be empty";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var projectData = new ProjectPostDto { ProjectName = NewProjectName };
            var newProject = await _projectService.CreateAsync(projectData);
            if (newProject != null)
            {
                Projects.Add(newProject);
                NewProjectName = string.Empty;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create project: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteProjectAsync(ProjectGetDto? project)
    {
        if (project == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _projectService.DeleteAsync(project.ProjectId);
            if (success)
            {
                Projects.Remove(project);
                if (SelectedProject?.ProjectId == project.ProjectId)
                {
                    SelectedProject = null;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete project: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task SelectProjectAsync(ProjectGetDto? project)
    {
        if (project == null) return;

        SelectedProject = project;

        await Shell.Current.GoToAsync(nameof(ProjectView), new Dictionary<string, object>
        {
            { "Project", project }
        });
    }

    [RelayCommand]
    private async Task UpdateProjectAsync(ProjectGetDto project)
    {
        if (project == null || string.IsNullOrWhiteSpace(project.ProjectName))
        {
            ErrorMessage = "Invalid project data";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var projectData = new ProjectPutDto { ProjectId = project.ProjectId, ProjectName = project.ProjectName };
            var updatedProject = await _projectService.UpdateAsync(projectData);
            if (updatedProject != null)
            {
                var index = Projects.IndexOf(project);
                if (index >= 0)
                {
                    Projects[index] = updatedProject;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update project: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddUserToProjectAsync(string email)
    {
        if (SelectedProject == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _projectService.AddUserToProjectAsync(SelectedProject.ProjectId, email);
            if (!success)
            {
                ErrorMessage = "Failed to add user to project";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to add user: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RemoveUserFromProjectAsync(string email)
    {
        if (SelectedProject == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _projectService.RemoveUserFromProjectAsync(SelectedProject.ProjectId, email);
            if (!success)
            {
                ErrorMessage = "Failed to remove user from project";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to remove user: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task MakeUserAdminAsync(Guid userId)
    {
        if (SelectedProject == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _projectService.MakeUserAdminAsync(SelectedProject.ProjectId, userId);
            if (!success)
            {
                ErrorMessage = "Failed to make user admin";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update user permissions: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ManageProjectUsersAsync(ProjectGetDto? project)
    {
        if (project == null) return;

        var viewModel = new ManageProjectUsersViewModel(_projectService, project);
        var popup = new ManageProjectUsersPopup(viewModel);
        await Shell.Current.Navigation.PushModalAsync(popup);
    }
}