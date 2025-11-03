using Desktop.Views;

namespace Desktop
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
            Routing.RegisterRoute(nameof(ProjectView), typeof(ProjectView));
        }
    }
}
