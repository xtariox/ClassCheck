using ClassCheck.Views;
using MauiApp1.Views;


namespace ClassCheck
{
    public partial class AppShell : Shell
    {
        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Register routes programmatically using DI
            RegisterRoutes(serviceProvider);

            // Set the current route to the login page
            CurrentItem = Items[0];
        }

        private void RegisterRoutes(IServiceProvider serviceProvider)
        {
            // Resolve pages via DI
            var loginPage = serviceProvider.GetRequiredService<LoginPage>();
            var registerPage = serviceProvider.GetRequiredService<RegisterPage>();
            var mainPage = serviceProvider.GetRequiredService<MainPage>();
            var addstudentPage = serviceProvider.GetRequiredService<AddStudentPage>();
            var homepage = serviceProvider.GetRequiredService<HomePage>();
            var addlesson = serviceProvider.GetRequiredService<AddLessonPage>();
            var attendance = serviceProvider.GetRequiredService<AttendancePage>();

            // Add ShellContent for each page
            this.Items.Add(new ShellContent
            {
                Route = "login",
                Content = loginPage
            });

            this.Items.Add(new ShellContent
            {
                Route = "register",
                Content = registerPage
            });
            this.Items.Add(new ShellContent
            {
                Route = "main",
                Content = mainPage
            });

            this.Items.Add(new ShellContent
            {
                Route = "addstudent",
                Content = addstudentPage
            });
            this.Items.Add(new ShellContent
            {
                Route = "home",
                Content = homepage
            });
            this.Items.Add(new ShellContent
            {
                Route = "addlesson",
                Content = addlesson
            });

            this.Items.Add(new ShellContent
            {
                Route = "attendance",
                Content = attendance
            });

        }
    }
}