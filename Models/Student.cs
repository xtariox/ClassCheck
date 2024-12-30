using SQLite;

namespace ClassCheck.Models
{
    public class Student : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Set as primary key and auto-incremented

        [Unique, NotNull]
        public string IDCardNumber { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName { get; set; }

        [Unique, NotNull]
        public string Email { get; set; }

        [NotNull]
        public string PhoneNumber { get; set; }

        [NotNull]
        public string Major { get; set; } // Foreign key for Major

    }
}
