using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public class JobSkill
    {
        #region Foreign Keys
        public int SkillId { get; set; }
        public Skill Skill { get; set; } 

        public int JobId { get; set; }
        public Job Job { get; set; }
        #endregion
    }
}
