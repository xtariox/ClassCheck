using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClassCheck.Models
{
    public class Session : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public int UserId { get; set; }

        [NotNull]
        public string UserName { get; set; }
    }
}
