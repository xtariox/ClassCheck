using ClassCheck.Models;
using SQLite;

namespace ClassCheck.Services
{
    public class DatabaseService
    {
        private const string DB_NAME = "attendanceDB.db3";
        private readonly SQLiteAsyncConnection _connection;
        private readonly SecurityService _securityService;
        private static TaskCompletionSource<bool> _initializationCompleted = new();
        private bool _majorsInitialized = false;

        public DatabaseService(SecurityService securityService)
        {
            _securityService = securityService;
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DB_NAME);
            _connection = new SQLiteAsyncConnection(dbPath);
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            //await ResetDatabaseAsync();
            try
            {
                await InitializeDatabaseAsync();
                _initializationCompleted.SetResult(true);
            }
            catch (Exception ex)
            {
                _initializationCompleted.SetException(ex);
            }
        }

        // Helper method to ensure database is initialized
        private async Task EnsureInitializedAsync()
        {
            await _initializationCompleted.Task;
        }

        // Initialize the database asynchronously without blocking
        public async Task InitializeDatabaseAsync()
        {
            // Create tables
            await _connection.CreateTableAsync<User>();
            await _connection.CreateTableAsync<Student>();
            await _connection.CreateTableAsync<Lesson>();
            await _connection.CreateTableAsync<Major>();
            await _connection.CreateTableAsync<Attendance>();

            // Create indices
            await CreateIndicesAsync();
            await InitializeMajorsAsync();

        }
        // ------------------ User -----------------------
        // Ensure email uniqueness by creating an index
        private async Task CreateIndicesAsync()
        {
            // Execute raw SQL to create indices if needed
            string createEmailIndex = "CREATE UNIQUE INDEX IF NOT EXISTS idx_email ON Users (Email);";
            await _connection.ExecuteAsync(createEmailIndex);
        }

        // ------------------ Generic CRUD Operations -----------------------
        public async Task<List<T>> GetAllAsync<T>() where T : IEntity, new()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : IEntity, new()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> InsertAsync<T>(T entity) where T : IEntity, new()
        {
            await EnsureInitializedAsync();
            return await _connection.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync<T>(T entity) where T : IEntity, new()
        {
            await EnsureInitializedAsync();
            return await _connection.UpdateAsync(entity);
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : IEntity, new()
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(entity);
        }
        // ---------------------------------------------------------------

        // ------------------ Retain Specialized Methods -------------------
        public async Task<User> GetByEmailAsync(string email)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Reintroduced method to get a student by ID card number
        public async Task<Student> GetByIDCardNumberAsync(string idCardNumber)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Student>().Where(s => s.IDCardNumber == idCardNumber).FirstOrDefaultAsync();
        }

        // Method to get all attendance
        public async Task<List<Attendance>> GetAttendanceByFiltersAsync(int lessonId, string major, DateTime date)
        {
            await EnsureInitializedAsync();
            var query = _connection.Table<Attendance>().Where(a => a.LessonId == lessonId && a.Major == major && a.AttendanceDate.Date == date.Date);
            return await query.ToListAsync();
        }

        // Method to get attendance by student
        public async Task<int> UpdateAttendanceAsync(Attendance attendance)
        {
            await EnsureInitializedAsync();
            return await _connection.UpdateAsync(attendance);
        }
        // ---------------------------------------------------------------

        // Method to initialize the database with predefined majors
        public async Task InitializeMajorsAsync()
        {
            if (_majorsInitialized) return;
        
            await _connection.RunInTransactionAsync(async (db) =>
            {
                var count = db.Table<Major>().Count();
                if (count == 0)
                {
                    var majors = new List<Major>
                    {
                        new Major { Name = "Computer Science" },
                        new Major { Name = "Engineering" },
                        new Major { Name = "Business" }
                    };
                    foreach (var major in majors)
                    {
                        db.Insert(major);
                    }
                }
                _majorsInitialized = true;
            });
        }

        // Method to get all majors
        public async Task<List<Major>> GetMajorsAsync()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Major>().ToListAsync();
        }


        // Method to reset the database
        public async Task ResetDatabaseAsync()
        {
            await _connection.DropTableAsync<User>();
            await _connection.DropTableAsync<Student>();
            await _connection.DropTableAsync<Lesson>();
            await _connection.DropTableAsync<Major>();
            await _connection.DropTableAsync<Attendance>();

            _majorsInitialized = false;
            _initializationCompleted = new TaskCompletionSource<bool>();

            await InitializeAsync();
        }
    }
}
