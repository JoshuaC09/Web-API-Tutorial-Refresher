using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeApp.Model
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(250)]
        [Required]
        public string Name { get; set; }
        [MaxLength(150)]
        [Required]
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
    }
}
