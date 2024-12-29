using System;

namespace MauiApp1.Models
{
    public class Attendance
    {
        public int Id { get; set; }              // Unique ID for each attendance record
        public string StudentId { get; set; }   // Foreign key for the student
        public string LessonId { get; set; }    // Foreign key for the lesson
        public string Major { get; set; }
        public bool IsPresent { get; set; }     // Whether the student is present
        public DateTime AttendanceDate { get; set; } // Date of attendance
    }
}
