using RecruitmentTest.Domain.Interfaces;
using RecruitmentTest.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<ApplicationUser> ApplicationUsers { get; }
        IBaseRepository<ApplicationUserJob> ApplicationUsersJobs { get; }
        IBaseRepository<Category> Categories { get; }
        IBaseRepository<Job> Jobs { get; }
        IBaseRepository<JobResponsability> JobsResponsabilities { get; }
        IBaseRepository<JobSkill> JobsSkills{ get; }
        IBaseRepository<Responsability> Responsabilities { get; }
        IBaseRepository<Skill> Skills { get; }

        int Complete();
    }
}
