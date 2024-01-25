using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public  class Job
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int MaxApplicants { get; set; }
        #endregion
        #region Foreign Keys
        public int CategoryId { get; set; }
        public Category Category { get; set; } 
        #endregion
        #region Navigation Properties
        public ICollection<JobSkill> JobSkills { get; set; } 
        public ICollection<JobResponsability> JobResponsabilities { get; set; } 
        public ICollection<ApplicationUserJob> JobApplicationUsers { get; set; } 
        #endregion
    }
}
