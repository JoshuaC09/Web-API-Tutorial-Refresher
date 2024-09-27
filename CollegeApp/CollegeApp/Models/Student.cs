using CollegeApp.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeApp.Model
{
    public class Student
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; } // Make it nullable
        public DateTime DOB { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
    }
}
