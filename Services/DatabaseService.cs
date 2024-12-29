using ClassCheck.Models;
using MauiApp1.Models;
using SQLite;

namespace ClassCheck.Services
{
    public class DatabaseService
    {
        private const string DB_NAME = "attendanceDB.db3";
        private readonly SQLiteAsyncConnection _connection;
        private readonly SecurityService _securityService;

        public DatabaseService(SecurityService securityService)
        {
            _securityService = securityService;
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            InitializeDatabaseAsync().Wait(); // Ensure initialization is completed
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
            return await _connection.Table<User>().ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _connection.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _connection.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<int> Insert(User user)
        {
            return await _connection.InsertAsync(user);
        }

        public async Task<int> Update(User user)
        {
            return await _connection.UpdateAsync(user);
        }

        public async Task<int> Delete(User user)
        {
            return await _connection.DeleteAsync(user);
        }



        // -------------------- Student ---------------------------

        public async Task<List<Student>> GetStudents()
        {
            return await _connection.Table<Student>().ToListAsync();
        }

        public async Task<Student> GetByIDCardNumber(String idCardNumber)
        {
            return await _connection.Table<Student>().Where(s => s.IDCardNumber == idCardNumber).FirstOrDefaultAsync();
        }



        public async Task<int> Insert(Student student)
        {
            return await _connection.InsertAsync(student);
        }

        public async Task<int> Update(Student student)
        {
            return await _connection.UpdateAsync(student);
        }

        public async Task<int> Delete(Student student)
        {
            return await _connection.DeleteAsync(student);
        }




        // -------------------- Lesson ---------------------------

        public async Task<List<Lesson>> GetLessons()
        {
            return await _connection.Table<Lesson>().ToListAsync();
        }

        public async Task<Lesson> GetByID(int id)
        {
            return await _connection.Table<Lesson>().Where(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> Insert(Lesson lesson)
        {
            return await _connection.InsertAsync(lesson);
        }

        public async Task<int> Update(Lesson lesson)
        {
            return await _connection.UpdateAsync(lesson);
        }

        public async Task<int> Delete(Lesson lesson)
        {
            return await _connection.DeleteAsync(lesson);
        }

        // -------------------- Major ---------------------------
        // Method to add a major
        // Method to initialize the database with predefined majors
        public async Task InitializeMajorsAsync()
        {
            var count = await _connection.Table<Major>().CountAsync();
            if (count == 0)
            {
                var majors = new List<Major>{
                    new Major {Name = "Computer Science" },
                    new Major {Name = "Engineering" },
                    new Major {Name = "Business" }
                };
                foreach (var major in majors)
                {
                    await _connection.InsertAsync(major);
                }
            }
        }

        // Method to get all majors
        public async Task<List<Major>> GetMajorsAsync()
        {
            return await _connection.Table<Major>().ToListAsync();
        }


        // -------------------- Attendance ---------------------------
        // Method to add an attendance
        public async Task<int> AddAttendanceAsync(Attendance attendance)
        {
            return await _connection.InsertAsync(attendance);
        }

        // Method to get all attendance
        public async Task<List<Attendance>> GetAttendanceByFiltersAsync(string lessonId, string major, DateTime date)
        {
            var query = _connection.Table<Attendance>().Where(a => a.LessonId == lessonId && a.Major == major && a.AttendanceDate.Date == date.Date);
            return await query.ToListAsync();
        }

        // Method to get attendance by student
        public async Task<int> UpdateAttendanceAsync(Attendance attendance)
        {
            return await _connection.UpdateAsync(attendance);
        }




    }
}
