using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CollegeApp.Model
{
    public class StudentDTO
    {
        [ValidateNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please don't forget to input your name")]
        [StringLength(30)]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Email address is not in correct format")]
        //Use this [Remote] attribute for checking email if already exist
        public string Email { get; set; }
        [Required]
        [Range(10000, 100000)]
        public int Allowance { get; set; }

        
        public string Password { get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public DateTime AdmissionDate { get; set; }


    }
}
