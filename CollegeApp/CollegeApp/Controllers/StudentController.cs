using CollegeApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController:ControllerBase
    {
        [HttpGet]
        [Route("All",Name = "GetAllStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
      
        public ActionResult<IEnumerable<Student>> GetStudentName()
        {
            return Ok(CollegeRepository.Students);
        }

        [HttpGet ("{id:int}",Name ="GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Student> GetStudent(int id)
        {

            if(id <= 0)
            {
                return BadRequest("Invalid Id");
            }
            Student student = CollegeRepository.Students.Where(item => item.Id == id).FirstOrDefault();
            if (student == null)
            {
                return NotFound($"Student {id} not exist int the database");
            }
            return Ok(student);
        }

        [HttpDelete]
        [Route("{id:int:min(1):max(100)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<bool> DeleteStudent(int id)
        {

            Student student = CollegeRepository.Students.Where(student => student.Id == id).FirstOrDefault();

            if(student == null)
            {
                return NotFound($"Student with {id} is not found");
            }
            CollegeRepository.Students.Remove(student);

            return Ok(true);
        }
    }
}
