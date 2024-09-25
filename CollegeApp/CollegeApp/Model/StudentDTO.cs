using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Model
{
    public class StudentDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public int Allowance { get; set; }
    }
}
