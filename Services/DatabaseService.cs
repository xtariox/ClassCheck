using ClassCheck.Models;
using ClassCheck.Models;
using SQLite;

namespace ClassCheck.Services
{
    public class DatabaseService
    {
        private const string DB_NAME = "attendanceDB.db3";
        private readonly SQLiteAsyncConnection _connection;
        private readonly SecurityService _securityService;
        private static readonly TaskCompletionSource<bool> _initializationCompleted = new();
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

        public async Task<List<User>> GetUsers()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<User>().ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<int> Insert(User user)
        {
            await EnsureInitializedAsync();
            return await _connection.InsertAsync(user);
        }

        public async Task<int> Update(User user)
        {
            await EnsureInitializedAsync();
            return await _connection.UpdateAsync(user);
        }

        public async Task<int> Delete(User user)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(user);
        }



        // -------------------- Student ---------------------------

        public async Task<List<Student>> GetStudents()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Student>().ToListAsync();
        }

        public async Task<Student> GetByIDCardNumber(string idCardNumber)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Student>().Where(s => s.IDCardNumber == idCardNumber).FirstOrDefaultAsync();
        }

        public async Task<int> Insert(Student student)
        {
            await EnsureInitializedAsync();
            return await _connection.InsertAsync(student);
        }

        public async Task<int> Update(Student student)
        {
            await EnsureInitializedAsync();
            return await _connection.UpdateAsync(student);
        }

        public async Task<int> Delete(Student student)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(student);
        }




        // -------------------- Lesson ---------------------------

        public async Task<List<Lesson>> GetLessons()
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Lesson>().ToListAsync();
        }

        public async Task<Lesson> GetByID(int id)
        {
            await EnsureInitializedAsync();
            return await _connection.Table<Lesson>().Where(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> Insert(Lesson lesson)
        {
            await EnsureInitializedAsync();
            return await _connection.InsertAsync(lesson);
        }

        public async Task<int> Update(Lesson lesson)
        {
            await EnsureInitializedAsync();
            return await _connection.UpdateAsync(lesson);
        }

        public async Task<int> Delete(Lesson lesson)
        {
            await EnsureInitializedAsync();
            return await _connection.DeleteAsync(lesson);
        }

        // -------------------- Major ---------------------------
        // Method to add a major
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


        // -------------------- Attendance ---------------------------
        // Method to add an attendance
        public async Task<int> AddAttendanceAsync(Attendance attendance)
        {
            await EnsureInitializedAsync();
            return await _connection.InsertAsync(attendance);
        }

        // Method to get all attendance
        public async Task<List<Attendance>> GetAttendanceByFiltersAsync(string lessonId, string major, DateTime date)
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
    }
}
