using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public class ApplicationUserJob
    {
        #region Foreign Keys
        public string ApplicationUserId { get; set; } 
        public ApplicationUser ApplicationUser { get; set; } 

        public int JobId { get; set; }
        public Job Job { get; set; }
        #endregion
    }
}
