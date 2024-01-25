using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public class JobResponsability
    {
        #region Foreign Keys
        public int ResponsabilityId { get; set; }
        public Responsability Responsability { get; set; } 

        public int JobId { get; set; }
        public Job Job { get; set; } 
        #endregion
    }
}
