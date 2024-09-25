using CollegeApp.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
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

            IEnumerable<StudentDTO> students = CollegeRepository.Students.Select(item => new StudentDTO()
            {
                Id = item.Id,
                Name = item.Name,
                Email = item.Email,
                Allowance = item.Allowance,
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

            if (student == null)
            {
                return NotFound($"Student with {id} is not found");
            }
            CollegeRepository.Students.Remove(student);

            return Ok(true);
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> CreateStudent([FromBody]StudentDTO studentModel)
        {
            //Use this if  [ApiController]  is not present or commented
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(studentModel == null)
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
            
           

            int newId = CollegeRepository.Students.LastOrDefault().Id + 1;

            Student student = new Student()
            {
                Id = newId,
                Name = studentModel.Name,
                Email = studentModel.Email,
                Allowance = studentModel.Allowance,
            };

            CollegeRepository.Students.Add(student);

            studentModel.Id = newId;

            return CreatedAtRoute("GetStudentById", new {Id =newId},studentModel);
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<StudentDTO> UpdateStudent([FromBody] StudentDTO model)
        {
            if(model == null ||model.Id <=0)
            {
                BadRequest();
            }
            Student existingStudent = CollegeRepository.Students.Where(student => student.Id == model.Id).FirstOrDefault();
            if(existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.Name = model.Name; 
            existingStudent.Email = model.Email;
            existingStudent.Allowance = model.Allowance;    

            return NoContent();
        }


        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<StudentDTO> UpdateStudentPartial(int id,[FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
           
             if(patchDocument == null ||id <=0)
            {
                BadRequest();
            }
            Student existingStudent = CollegeRepository.Students.Where(student => student.Id == id).FirstOrDefault();
            if (existingStudent == null)
            {
                return NotFound();
            }

            StudentDTO studentDTO = new StudentDTO()
            {
                Id = existingStudent.Id,
                Name = existingStudent.Name,
                Email = existingStudent.Email,
                Allowance = existingStudent.Allowance,
            };

            patchDocument.ApplyTo(studentDTO,ModelState);

            if (!ModelState.IsValid) 
            { 
              return BadRequest();
            }

            existingStudent.Name = studentDTO.Name;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Allowance = studentDTO.Allowance;
            return NoContent();
        }



    }
}
