using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class Lesson
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public string CourseName { get; set; }
        [NotNull]
        public string Professor { get; set; }
        [NotNull]
        public string Schedule { get; set; }
        [NotNull]
        public string Major { get; set; }

    }
}
