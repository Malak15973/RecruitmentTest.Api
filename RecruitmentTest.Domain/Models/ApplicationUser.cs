using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        #region Properties
        public string Name { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        #endregion
        #region Navigation Properties
        public ICollection<ApplicationUserJob> ApplicationUserJobs { get; set; } 
        #endregion
    }
}
