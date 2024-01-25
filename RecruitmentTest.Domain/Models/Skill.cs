using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public class Skill
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; } 
        #endregion
        #region Navigation Properties
        public ICollection<JobSkill> JobsSkills { get; set; } 
        #endregion
    }
}
