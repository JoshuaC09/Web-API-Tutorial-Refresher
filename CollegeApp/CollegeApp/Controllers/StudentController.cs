using AutoMapper;
using CollegeApp.Data.Repository;
using CollegeApp.Model;
using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "LoginForLocalUsers", Roles ="Superadmin, Admin")]
   

    //[EnableCors(PolicyName = "AllowOnlyLocalHost")] //Controller Or Class level CORS
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;
        //private readonly IGenericRepository<Student> _studentRepository;
        private readonly IStudentRepository _studentRepository;
        private APIResponse _apiResponse;


        public StudentController(ILogger<StudentController> logger, IMapper mapper, IStudentRepository studentRepository)
        {
            _logger = logger;
            _studentRepository = studentRepository;
            _mapper = mapper;
            _apiResponse = new APIResponse();
        }

        [HttpGet]
        [Route("All", Name = "GetAllStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
      

        public async Task<ActionResult<APIResponse>> GetAllStudentAsync()
        {

            try
            {
                _logger.LogInformation("GetStudents method started..");

                List<Student> students = await _studentRepository.GetAllAsync();

                _apiResponse.Data = _mapper.Map<List<StudentDTO>>(students);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex) 
            { 
             _apiResponse.Errors.Add(ex.Message);
             _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
             _apiResponse.Status = false;
                return _apiResponse;
            }

        } 

        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [EnableCors(PolicyName ="AllowAll")] // Method level CORS
      

        public async Task<ActionResult<APIResponse>> GetStudentByIdAsync(int id)
        {
            try
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

                _apiResponse.Data = _mapper.Map<StudentDTO>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

        [HttpDelete]
        [Route("{id:int:min(1):max(100)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[DisableCors] //To Disable CORS

        public async Task<ActionResult<APIResponse>> DeleteStudent(int id)
        {
            try
            {
                Student student = await _studentRepository.GetAsync(student => student.Id == id, true);

                if (student == null)
                {
                    return NotFound($"Student with {id} is not found");
                }
                await _studentRepository.DeleteAsync(student);

                _apiResponse.Data = true;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }


        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<ActionResult<APIResponse>> CreateStudentAsync([FromBody] StudentDTO studentModel)
        {
            try
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


                if (string.IsNullOrEmpty(student.Address))
                {
                    student.Address = "Default Address";
                }

                var studentAfterCreation = await _studentRepository.CreateAsync(student);

                studentModel.Id = studentAfterCreation.Id;
                _apiResponse.Data = studentModel;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return CreatedAtRoute("GetStudentById", new { Id = studentModel.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> UpdateStudent([FromBody] StudentDTO studentDTO)
        {
            try
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
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }


        [HttpPatch]
        [Route("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            try
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
            catch (Exception ex)
            {
                _apiResponse.Errors.Add(ex.Message);
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Status = false;
                return _apiResponse;
            }
        }

    }
}