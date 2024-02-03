using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Dtos.Account
{
    public class TokenDto
    {
        [Required(ErrorMessage = "Refresh Token Is Required")]
        public string RefreshToken { get; set; }
    }
}
