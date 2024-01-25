using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Dtos.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage ="Name Is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Email Is Invalid")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Is Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "MobileNumber Is Required")]
        [Phone(ErrorMessage = "Mobile Number Is Invalid")]
        public string MobileNumber { get; set; }

    }
}
