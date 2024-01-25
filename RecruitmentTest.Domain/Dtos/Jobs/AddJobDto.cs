using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Dtos.Jobs
{
    public class AddJobDto
    {
        [Required(ErrorMessage ="Job Name Is Required")]
        public string Name { get; set; } 
        [Required(ErrorMessage = "Job Description Is Required")]
        public string Description { get; set; } 
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        [Range(1,int.MaxValue,ErrorMessage = "Please Enter Range >= 1")]
        public int MaxApplicants { get; set; }
        public int CategoryId { get; set; }
        public List<int> JobSkills { get; set; }
        public List<int> JobResponsabilities { get; set; }
    }
}
