using ClassCheck.Services;
using ClassCheck.ViewModels;
using ClassCheck.Views;
using ClassCheck.ViewModels;
using ClassCheck.Views;
using Microsoft.Extensions.Logging;


namespace ClassCheck
{
    public partial class App : Application
    {
        public static DatabaseService? Database { get; private set; }

        public App()
        {
            InitializeComponent();

            // Initialize Dependency Injection (DI) container
            var services = new ServiceCollection();

            // Configure logging
            services.AddLogging(configure =>
            {
                configure.AddDebug();
                configure.SetMinimumLevel(LogLevel.Debug);
            });

            // Register services with DI container
            // Use AddSingleton to ensure that only one instance of each service is created
            services.AddSingleton<SecurityService>();
            services.AddSingleton<DatabaseService>(provider =>
                new DatabaseService(provider.GetRequiredService<SecurityService>()));
            services.AddSingleton<UserService>();

            // Register ViewModels
            // Use AddTransient to create a new instance of the ViewModel each time it is requested
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<AddStudentViewModel>();
            services.AddTransient<MajorViewModel>();
            services.AddTransient<HomePageViewModel>();
            services.AddTransient<AddLessonViewModel>();
            services.AddTransient<AttendanceViewModel>();


            // Register Pages
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<MainPage>();
            services.AddTransient<AddStudentPage>();
            services.AddTransient<HomePage>();
            services.AddTransient<AddLessonPage>();
            services.AddTransient<AttendancePage>();

            // Register AppShell
            services.AddSingleton<AppShell>(); // Main application shell which is responsible for navigation and routing

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();
            DependencyService = serviceProvider; // Set the DependencyService instance for later use

            // Register services
            //DependencyService.Register<DatabaseService>();
            //DependencyService.Register<MajorViewModel>();

            // Set the MainPage to AppShell
            MainPage = serviceProvider.GetRequiredService<AppShell>();

            // Run database initialization asynchronously in the background
            InitializeDatabaseAsync();
        }

        // The DependencyService instance that is set up for DI.
        public static IServiceProvider? DependencyService { get; private set; }

        private static async void InitializeDatabaseAsync()
        {
            try
            {
                // Access the database service from the DI container
                Database = DependencyService?.GetService<DatabaseService>();

                // Initialize the database asynchronously
                await Database?.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                // Handle database initialization errors gracefully
                Console.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }
    }
}
