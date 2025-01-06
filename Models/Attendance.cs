using System;
using System.ComponentModel.DataAnnotations;
using SQLite;

namespace ClassCheck.Models
{
    public class Attendance : IEntity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        // Unique ID for each attendance record
        [NotNull]
        public int StudentId { get; set; }   // Foreign key for the student
        [NotNull]
        public int LessonId { get; set; }    // Foreign key for the lesson
        [NotNull]
        public string Major { get; set; }
        [NotNull]
        public bool IsPresent { get; set; }     // Whether the student is present
        [NotNull]
        public string StudentFName { get; set; }
        [NotNull]
        public string StudentLName { get; set; }
        [NotNull]
        public DateTime AttendanceDate { get; set; } // Date of attendance
    }
}
