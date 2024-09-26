using CollegeApp.Data;
using CollegeApp.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly CollegeDbContext _context;

        public StudentController(ILogger<StudentController> logger,CollegeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<IEnumerable<StudentDTO>> GetStudentName()
        {
            //List<StudentDTO> students = new List<StudentDTO>();
            //foreach (Student item in CollegeRepository.Students)
            //{
            //    StudentDTO student = new StudentDTO() { 
            //    Id = item.Id,
            //    Name = item.Name,
            //     Email = item.Email,
            //     Allowance = item.Allowance,
            //    };
            //    students.Add(student);
            //}

            _logger.LogInformation("GetStudents method started..");

            IEnumerable<StudentDTO> students = _context.Students.Select(item => new StudentDTO()
            {
                Id = item.Id,
                Name = item.Name,
                Email = item.Email,
                DateTime = item.DateTime,   
                
               
            }).ToList();


            return Ok(students);
        }


        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Student> GetStudent(int id)
        {

            if (id <= 0)
            {
                _logger.LogWarning("BadRequest");
                return BadRequest("Invalid Id");
            }

            Student student = _context.Students.Where(item => item.Id == id).FirstOrDefault();


            if (student == null)
            {
                return NotFound($"Student {id} not exist int the database");
            }

            var studentDTO = new StudentDTO()
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                DateTime = student.DateTime,
            };


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

            Student student = _context.Students.Where(student => student.Id == id).FirstOrDefault();

            if (student == null)
            {
                return NotFound($"Student with {id} is not found");
            }
            _context.Students.Remove(student);
            _context.SaveChanges();
            return Ok(true);
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> CreateStudent([FromBody] StudentDTO studentModel)
        {
            //Use this if  [ApiController]  is not present or commented
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (studentModel == null)
            {
                return BadRequest();
            }

            //if(studentModel.AdmissionDate <= DateTime.Now)
            //{
            //    //1. Directly adding error message to model state
            //    //2. Using custom attribute
            //    ModelState.AddModelError("AdmissionDate Error","Admission date must be greater than  or equal to todays date");
            //    return BadRequest(ModelState);
            //}

            Student student = new Student() {
             Id = studentModel.Id,
             Name = studentModel.Name,
             Email = studentModel.Email,
             DateTime = studentModel.DateTime,
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return CreatedAtRoute("GetStudentById", new {Id=studentModel.Id }, studentModel);
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<StudentDTO> UpdateStudent([FromBody] StudentDTO model)
        {
            if (model == null || model.Id <= 0)
            {
                BadRequest();
            }
            Student existingStudent = _context.Students.AsNoTracking().Where(student => student.Id == model.Id).FirstOrDefault();
            if (existingStudent == null)
            {
                return NotFound();
            }

            var newStudent = new Student()
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                DateTime = model.DateTime,
            };

            _context.Students.Update(newStudent);

            //existingStudent.Name = model.Name;
            //existingStudent.Email = model.Email;
            //existingStudent.DateTime = model.DateTime;

            _context.SaveChanges();
            return NoContent();
        }


        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            // Validate patch document and id
            if (patchDocument == null || id <= 0)
            {
                return BadRequest("Invalid patch document or ID.");
            }

            // Retrieve existing student from the database
            var existingStudent = _context.Students.FirstOrDefault(student => student.Id == id);
            if (existingStudent == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            // Map existing student to a DTO
            var studentDTO = new StudentDTO()
            {
                Id = existingStudent.Id,
                Name = existingStudent.Name,
                Email = existingStudent.Email,
                DateTime = existingStudent.DateTime,
            };

            // Apply the patch document to the DTO
            patchDocument.ApplyTo(studentDTO);

            // Validate the patched DTO
            if (!TryValidateModel(studentDTO))
            {
                return BadRequest(ModelState);
            }

            // Update the existing student with the patched values
            existingStudent.Name = studentDTO.Name;
            existingStudent.Email = studentDTO.Email;
            existingStudent.DateTime = studentDTO.DateTime;

                _context.SaveChanges();

            return NoContent();
        }

    }
}