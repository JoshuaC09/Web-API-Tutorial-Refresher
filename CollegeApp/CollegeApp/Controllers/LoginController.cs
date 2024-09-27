using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public ActionResult Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide username and password");
            }

            var jwtKeyForGoogle = _configuration["Jwt:KeyForGoogle"];
            var jwtKeyForMicrosoft = _configuration["Jwt:KeyForMicrosoft"];
            var jwtKeyForLocalUser = _configuration["Jwt:KeyForLocal"];

            var googleAudience = _configuration["Jwt:GoogleAudience"];
            var microsoftAudience = _configuration["Jwt:MicrosoftAudience"];
            var localAudience = _configuration["Jwt:localAudience"];

            var googleIssuer = _configuration["Jwt:GoogleIssuer"];
            var microsoftIssuer = _configuration["Jwt:MicrosoftIssuer"];
            var localIssuer = _configuration["Jwt:localIssuer"];

            LoginResponseDTO response = new LoginResponseDTO() { UserName = "Joshua" };
            string jwtKey = null;
            string jwtIssuer = string.Empty;
            string jwtAudience = string.Empty;
            if (model.PolicyName == "Local")
            {
                jwtIssuer = localIssuer;
                jwtAudience = localAudience;
                jwtKey = jwtKeyForLocalUser;
            }
            else if (model.PolicyName == "Microsoft") {
                jwtIssuer = microsoftIssuer;
                jwtAudience = microsoftAudience;
                jwtKey = jwtKeyForMicrosoft;
             
            }
            else if (model.PolicyName == "Google") {

                jwtIssuer = googleIssuer;
                jwtAudience = googleAudience;
                jwtKey = jwtKeyForGoogle;
            }

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
            {
                return StatusCode(500, "JWT configuration is missing");
            }

            if (model.UserName == "Joshua" && model.Password == "123")
            {
                var key = Encoding.ASCII.GetBytes(jwtKey);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {

                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "Admin")
                    }),
                    Expires = DateTime.UtcNow.AddHours(4),
                    Issuer = jwtIssuer,
                    Audience = jwtAudience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.Token = tokenHandler.WriteToken(token);
            }
            else
            {
                return Unauthorized("Invalid username and password");
            }

            return Ok(response);
        }
    }
}