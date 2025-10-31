using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop.Services;
using Entities.DTO;
using System.Collections.ObjectModel;

namespace Desktop.ViewModels;

public partial class ProjectViewModel : ObservableObject
{
    private readonly IProjectService _projectService;
    private readonly ICategoryService _categoryService;
    private readonly ITaskService _taskService;

    [ObservableProperty]
    private ProjectGetDto? _project;

    [ObservableProperty]
    private ObservableCollection<CategoryGetDto> _categories = new();

    [ObservableProperty]
    private CategoryGetDto? _selectedCategory;

    [ObservableProperty]
    private string _newCategoryName = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ProjectViewModel(IProjectService projectService, ICategoryService categoryService, ITaskService taskService)
    {
        _projectService = projectService;
        _categoryService = categoryService;
        _taskService = taskService;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task LoadProjectAsync(Guid projectId)
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
                    Categories.Clear();
                    foreach (var category in project.Categories.OrderBy(c => c.SortOrder))
                    {
                        Categories.Add(category);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load project: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task RefreshProjectAsync()
    {
        if (Project != null)
        {
            await LoadProjectAsync(Project.ProjectId);
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task CreateCategoryAsync()
    {
        if (Project == null || string.IsNullOrWhiteSpace(NewCategoryName))
        {
            ErrorMessage = "Category name cannot be empty";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var categoryData = new CategoryPostDto { ProjectId = Project.ProjectId, CategoryName = NewCategoryName, SortOrder = Categories.Count };
            var newCategory = await _categoryService.CreateAsync(categoryData);
            if (newCategory != null)
            {
                Categories.Add(newCategory);
                NewCategoryName = string.Empty;
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
    private async System.Threading.Tasks.Task DeleteCategoryAsync(CategoryGetDto? category)
    {
        if (category == null) return;
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var success = await _categoryService.DeleteAsync(category.CategoryId);
            if (success)
            {
                Categories.Remove(category);
                if (SelectedCategory?.CategoryId == category.CategoryId)
                {
                    SelectedCategory = null;
                }
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

    [RelayCommand]
    private async System.Threading.Tasks.Task UpdateCategoryAsync(CategoryGetDto category)
    {
        if (category == null || string.IsNullOrWhiteSpace(category.CategoryName))
        {
            ErrorMessage = "Invalid category data";
            return;
        }
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var categoryData = new CategoryPutDto { CategoryId = category.CategoryId, ProjectId = category.ProjectId, CategoryName = category.CategoryName, SortOrder = category.SortOrder };
            var updatedCategory = await _categoryService.UpdateAsync(categoryData);
            if (updatedCategory != null)
            {
                var index = Categories.IndexOf(category);
                if (index >= 0)
                {
                    Categories[index] = updatedCategory;
                }
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
    private async System.Threading.Tasks.Task ReorderCategoriesAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                var category = Categories[i];
                if (category.SortOrder != i)
                {
                    var categoryData = new CategoryPutDto { CategoryId = category.CategoryId, ProjectId = category.ProjectId, CategoryName = category.CategoryName, SortOrder = i };
                    await _categoryService.UpdateAsync(categoryData);
                    category.SortOrder = i;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to reorder categories: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void MoveCategoryUp(CategoryGetDto? category)
    {
        if (category == null) return;
        var index = Categories.IndexOf(category);
        if (index > 0)
        {
            Categories.Move(index, index - 1);
        }
    }

    [RelayCommand]
    private void MoveCategoryDown(CategoryGetDto? category)
    {
        if (category == null) return;
        var index = Categories.IndexOf(category);
        if (index >= 0 && index < Categories.Count - 1)
        {
            Categories.Move(index, index + 1);
        }
    }
}
