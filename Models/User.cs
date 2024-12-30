using SQLite;

namespace ClassCheck.Models
{
    [Table("Users")]
    public class User : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Email { get; set; }

        [NotNull]
        public string Username { get; set; }

        [NotNull]
        public string Password { get; set; }
    }
}
