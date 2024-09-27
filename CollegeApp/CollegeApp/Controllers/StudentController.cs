﻿using AutoMapper;
using CollegeApp.Data.Repository;
using CollegeApp.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(PolicyName = "AllowOnlyLocalHost")] //Controller Or Class level CORS
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;
        //private readonly IGenericRepository<Student> _studentRepository;
        private readonly IStudentRepository _studentRepository;


        public StudentController(ILogger<StudentController> logger, IMapper mapper, IStudentRepository studentRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudentAsync()
        {


            _logger.LogInformation("GetStudents method started..");

            List<Student> students = await _studentRepository.GetAllAsync();

            List<StudentDTO> studentDTOData = _mapper.Map<List<StudentDTO>>(students);

            return Ok(studentDTOData);
        } 

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentByFeeStatusAsync(int feeStatus)
        {
            IEnumerable<Student> studentsWithFeeStatus = await _studentRepository.GetStudentsByFeeStatusAsync(feeStatus);

            return Ok(studentsWithFeeStatus);
        }


        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       // [EnableCors(PolicyName ="AllowAll")] // Method level CORS

        public async Task<ActionResult<Student>> GetStudentByIdAsync(int id)
        {

            if (id <= 0)
            {
                _logger.LogWarning("BadRequest");
                return BadRequest("Invalid Id");
            }

            Student student = await _studentRepository.GetAsync(student => student.Id == id);


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
        //[DisableCors] //To Disable CORS

        public async Task<ActionResult<bool>> DeleteStudent(int id)
        {

            Student student = await _studentRepository.GetAsync(student => student.Id == id, true);

            if (student == null)
            {
                return NotFound($"Student with {id} is not found");
            }
            await _studentRepository.DeleteAsync(student);
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

            await _studentRepository.CreateAsync(student);

            return CreatedAtRoute("GetStudentById", new { Id = studentModel.Id }, studentModel);
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
            Student existingStudent = await _studentRepository.GetAsync(student => student.Id == studentDTO.Id, true);
            if (existingStudent == null)
            {
                return NotFound();
            }

            Student newStudent = _mapper.Map<Student>(studentDTO);
            await _studentRepository.UpdateAsync(newStudent);
            return NoContent();
        }


        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                return BadRequest();
            }

            var existingStudent = await _studentRepository.GetAsync(student => student.Id == id);

            if (existingStudent == null)
            {
                return NotFound();
            }

            var studentDTO = _mapper.Map<StudentDTO>(existingStudent);

            patchDocument.ApplyTo(studentDTO, ModelState);

            if (!TryValidateModel(studentDTO))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(studentDTO, existingStudent);

             await _studentRepository.UpdateAsync(existingStudent);
           

            return NoContent();
        }

    }
}