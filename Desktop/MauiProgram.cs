using Microsoft.Extensions.Logging;
using Desktop.Services;
using Desktop.ViewModels;
using Desktop.Views;

namespace Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:7070")
                };
                return httpClient;
            });

            // Register Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IProjectService, ProjectService>();
            builder.Services.AddSingleton<ICategoryService, CategoryService>();
            builder.Services.AddSingleton<ITaskService, TaskService>();

            // Register ViewModels
            builder.Services.AddSingleton<AuthViewModel>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<ProjectListViewModel>();
            builder.Services.AddTransient<ProjectViewModel>();
            builder.Services.AddTransient<CategoryViewModel>();
            builder.Services.AddTransient<TaskDetailViewModel>();
            builder.Services.AddTransient<UserViewModel>();
            builder.Services.AddTransient<BoardViewModel>();

            // Register Views
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<RegisterView>();
            builder.Services.AddTransient<ProjectListView>();
            builder.Services.AddTransient<ProjectView>();

            return builder.Build();
        }
    }
}
