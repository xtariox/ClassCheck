using SQLite;

namespace ClassCheck.Models
{
    public class Student : IEntity
    {
        public int Id { get; set; } // Won't be used. It's just to implement IEntity

        [PrimaryKey]
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
