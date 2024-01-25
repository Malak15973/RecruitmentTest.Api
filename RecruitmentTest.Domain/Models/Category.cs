using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Models
{
    public class Category
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; } 
        #endregion
        #region Navigation Properties
        public ICollection<Job> Jobs { get; set; } 
        #endregion
    }
}
