using AutoMapper;
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
        private readonly IMapper _mapper;

        public StudentController(ILogger<StudentController> logger,CollegeDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudentAsync()
        {
            

            _logger.LogInformation("GetStudents method started..");

            List<Student> students = await _context.Students.ToListAsync();

            List<StudentDTO> studentDTOData = _mapper.Map<List<StudentDTO>>(students);

            return Ok(studentDTOData);
        }


        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Student>> GetStudentByIdAsync(int id)
        {

            if (id <= 0)
            {
                _logger.LogWarning("BadRequest");
                return BadRequest("Invalid Id");
            }

            Student student = await _context.Students.Where(item => item.Id == id).FirstOrDefaultAsync();


            if (student == null)
            {
                return NotFound($"Student {id} not exist int the database");
            }

            StudentDTO studentDTO = _mapper.Map<StudentDTO>(student);


            return Ok(studentDTO);
        }

        [HttpDelete]
        [Route("{id:int:min(1):max(100)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<bool>> DeleteStudent(int id)
        {

            Student student = _context.Students.Where(student => student.Id == id).FirstOrDefault();

            if (student == null)
            {
                return NotFound($"Student with {id} is not found");
            }
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok(true);
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentDTO>> CreateStudentAsync([FromBody] StudentDTO studentModel)
        {
           
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (studentModel == null)
            {
                return BadRequest();
            }


            Student student = _mapper.Map<Student>(studentModel);

            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetStudentById", new {Id=studentModel.Id }, studentModel);
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<StudentDTO>> UpdateStudent([FromBody] StudentDTO studentDTO)
        {
            if (studentDTO == null || studentDTO.Id <= 0)
            {
                BadRequest();
            }
            Student existingStudent = await _context.Students.AsNoTracking().Where(student => student.Id == studentDTO.Id).FirstOrDefaultAsync();
            if (existingStudent == null)
            {
                return NotFound();
            }

            Student newStudent = _mapper.Map<Student>(studentDTO);

            _context.Students.Update(newStudent);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
          
            if (patchDocument == null || id <= 0)
            {
                return BadRequest("Invalid patch document or ID.");
            }

        
            Student existingStudent = await _context.Students.AsNoTracking().FirstOrDefaultAsync(student => student.Id == id);
            if (existingStudent == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            StudentDTO studentDTO = _mapper.Map<StudentDTO>(existingStudent);
           
        
            patchDocument.ApplyTo(studentDTO);

            if (!TryValidateModel(studentDTO))
            {
                return BadRequest(ModelState);
            }

            existingStudent = _mapper.Map<Student>(studentDTO);

            _context.Students.Update(existingStudent);
             await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}