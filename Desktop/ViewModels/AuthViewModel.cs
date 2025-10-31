using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;

namespace Desktop.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isAuthenticated;

    [ObservableProperty]
    private UserGetDto? _currentUser;

    public AuthViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both email and password";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var loginData = new UserLogin { Email = Email, Password = Password };
            var response = await _authService.LoginAsync(loginData);
            if (response != null)
            {
                CurrentUser = new UserGetDto
                {
                    UserId = response.Id,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    Email = Email,
                    Password = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                IsAuthenticated = true;
                ErrorMessage = string.Empty;

                // Navigate to ProjectListView after successful login
                await Shell.Current.GoToAsync("//ProjectListView");
            }
            else
            {
                ErrorMessage = "Login failed. Please check your credentials.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
        string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "Please fill in all fields";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var registerData = new UserRegister { FirstName = FirstName, LastName = LastName, Email = Email, Password = Password };
            var response = await _authService.RegisterAsync(registerData);
            if (response != null)
            {
                CurrentUser = new UserGetDto
                {
                    UserId = response.Id,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    Email = Email,
                    Password = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                IsAuthenticated = true;
                ErrorMessage = string.Empty;

                // Navigate to ProjectListView after successful registration
                await Shell.Current.GoToAsync("//ProjectListView");
            }
            else
            {
                ErrorMessage = "Registration failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registration error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LogoutAsync()
    {
        _authService.ClearToken();
        IsAuthenticated = false;
        CurrentUser = null;
        Email = string.Empty;
        Password = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        ErrorMessage = string.Empty;

        // Navigate back to login
        await Shell.Current.GoToAsync("//LoginView");
    }
}
