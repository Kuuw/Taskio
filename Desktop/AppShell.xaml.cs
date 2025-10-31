using Desktop.Views;

namespace Desktop
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
            Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
            Routing.RegisterRoute(nameof(ProjectListView), typeof(ProjectListView));
            Routing.RegisterRoute(nameof(ProjectView), typeof(ProjectView));
        }
    }
}
