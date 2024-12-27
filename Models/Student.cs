using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    public class Student
    {
       
        [PrimaryKey]
        public string IDCardNumber { get; set; }
        [NotNull]
        public string FirstName { get; set; }
        [NotNull]
        public string LastName { get; set; }
        [Unique,NotNull]
        public string Email { get; set; }
        [NotNull]
        public string PhoneNumber { get; set; }
        [NotNull]
        public string Major { get; set; }


    }
}
