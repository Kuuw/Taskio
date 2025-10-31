using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IProjectService _projectService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private UserGetDto? _currentUser;

    [ObservableProperty]
    private bool _isAuthenticated;

    [ObservableProperty]
    private ObservableCollection<ProjectGetDto> _projects = new();

    [ObservableProperty]
    private ProjectGetDto? _selectedProject;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _currentView = "ProjectList";

    public MainViewModel(IProjectService projectService, IAuthService authService)
    {
        _projectService = projectService;
        _authService = authService;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task InitializeAsync()
    {
        if (IsAuthenticated)
        {
            await LoadProjectsAsync();
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoadProjectsAsync()
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
                StatusMessage = $"Loaded {projects.Count} projects";
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
    private void SelectProject(ProjectGetDto? project)
    {
        SelectedProject = project;
        if (project != null)
        {
            CurrentView = "ProjectDetail";
        }
    }

    [RelayCommand]
    private void NavigateToProjectList()
    {
        CurrentView = "ProjectList";
        SelectedProject = null;
    }

    [RelayCommand]
    private void NavigateToProfile()
    {
        CurrentView = "Profile";
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LogoutAsync()
    {
        _authService.ClearToken();
        IsAuthenticated = false;
        CurrentUser = null;
        Projects.Clear();
        SelectedProject = null;
        CurrentView = "Login";
        StatusMessage = "Logged out successfully";
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public void SetAuthenticatedUser(UserGetDto user)
    {
        CurrentUser = user;
        IsAuthenticated = true;
        CurrentView = "ProjectList";
        _ = LoadProjectsAsync();
    }

    public void ClearAuthentication()
    {
        CurrentUser = null;
        IsAuthenticated = false;
        Projects.Clear();
        SelectedProject = null;
        CurrentView = "Login";
    }
}
