using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Model
{
    public class StudentDTO
    {

      
        public int Id { get; set; }
  
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
    }
}
