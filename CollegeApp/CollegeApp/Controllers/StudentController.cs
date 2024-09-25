using CollegeApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController:ControllerBase
    {
        [HttpGet]
        public IEnumerable<Student> GetStudentName()
        {
            return CollegeRepository.Students;
        }

        [HttpGet ("{id:int}")]
        public Student GetStudent(int id)
        {
            return CollegeRepository.Students.Where(item => item.Id == id).FirstOrDefault();
        }
    }
}
