using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors(PolicyName ="AllowOnlyMicrosoft")]
    [Authorize(AuthenticationSchemes = "LoginForMicrosoftUsers",Roles ="Superadminm,Admin")]
    public class MicrosoftController : ControllerBase
    {
        [HttpGet]
        
        public ActionResult Get()
        {
            return Ok("This is microsoft");
        }
    }
}
