using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class UserViewModel : ObservableObject
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private ObservableCollection<UserGetDto> _users = new();

    [ObservableProperty]
    private UserGetDto? _selectedUser;

    [ObservableProperty]
    private UserGetDto? _currentUser;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<UserGetDto> _filteredUsers = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UserViewModel(IUserService userService)
    {
        _userService = userService;
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterUsers();
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var users = await _userService.GetAllAsync();
            if (users != null)
            {
                Users.Clear();
                FilteredUsers.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                    FilteredUsers.Add(user);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void FilterUsers()
    {
        FilteredUsers.Clear();
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var user in Users)
            {
                FilteredUsers.Add(user);
            }
        }
        else
        {
            var searchLower = SearchText.ToLower();
            foreach (var user in Users)
            {
                if (user.FirstName.ToLower().Contains(searchLower) ||
                user.LastName.ToLower().Contains(searchLower) ||
                user.Email.ToLower().Contains(searchLower))
                {
                    FilteredUsers.Add(user);
                }
            }
        }
    }

    [RelayCommand]
    private async Task UpdateUserAsync(UserGetDto user)
    {
        if (user == null)
        {
            ErrorMessage = "Invalid user data";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var userData = new UserPutDto { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, Password = user.Password };
            var updatedUser = await _userService.UpdateAsync(userData);
            if (updatedUser != null)
            {
                var index = Users.IndexOf(user);
                if (index >= 0)
                {
                    Users[index] = updatedUser;
                }
                var filteredIndex = FilteredUsers.IndexOf(user);
                if (filteredIndex >= 0)
                {
                    FilteredUsers[filteredIndex] = updatedUser;
                }
                if (CurrentUser?.UserId == user.UserId)
                {
                    CurrentUser = updatedUser;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to update user: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void SetCurrentUser(UserGetDto user)
    {
        CurrentUser = user;
    }
}
