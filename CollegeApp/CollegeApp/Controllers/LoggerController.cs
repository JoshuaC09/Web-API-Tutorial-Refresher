﻿using CollegeApp.MyLogging.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(PolicyName ="AllowOnlyGoolge")]
    public class LoggerController : ControllerBase
    {
        // 1. Strongly coupled/tightly coupled
        // 2. Loosely coupled

        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;

        private readonly IMyLogger _logger;
        public LoggerController(IMyLogger logger, IConfiguration configuration)
        { 
            _configuration = configuration;
            _jwtSecret = _configuration.GetSection("JWTSecret").Value;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult TestDI()
        {
           string rusult =  _logger.Log("Dependency Injection Tutorial");

            return Ok(_jwtSecret);
        }


    }
}
