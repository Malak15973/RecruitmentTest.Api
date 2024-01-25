using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Dtos.Jobs
{
    public class JobDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int MaxApplicants { get; set; }
        public bool IsOpen { get; set; }
        public int CategoryId { get; set; }
        public SelectListDto Category { get; set; }
        public ICollection<SelectListDto> JobSkills { get; set; }
        public ICollection<SelectListDto> JobResponsabilities { get; set; }
    }
}
