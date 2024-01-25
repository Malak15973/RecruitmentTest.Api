using Microsoft.EntityFrameworkCore.Infrastructure;
using RecruitmentTest.Domain.Interfaces;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecruitmentTest.DataAccess.Repositories;

namespace RecruitmentTest.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext db;
        public IBaseRepository<ApplicationUser> ApplicationUsers { get; private set; }
        public IBaseRepository<ApplicationUserJob> ApplicationUsersJobs { get; private set; }
        public IBaseRepository<Category> Categories { get; private set; }
        public IBaseRepository<Job> Jobs { get; private set; }
        public IBaseRepository<JobResponsability> JobsResponsabilities { get; private set; }
        public IBaseRepository<JobSkill> JobsSkills { get; private set; }
        public IBaseRepository<Responsability> Responsabilities { get; private set; }
        public IBaseRepository<Skill> Skills { get; private set; }

        public UnitOfWork(AppDbContext db)
        {
            this.db = db;

            ApplicationUsers = new BaseRepository<ApplicationUser>(db);
            ApplicationUsersJobs = new BaseRepository<ApplicationUserJob>(db);
            Categories = new BaseRepository<Category>(db);
            Jobs = new BaseRepository<Job>(db);
            JobsResponsabilities = new BaseRepository<JobResponsability>(db);
            JobsSkills = new BaseRepository<JobSkill>(db);
            Responsabilities = new BaseRepository<Responsability>(db);
            Skills = new BaseRepository<Skill>(db);
        }
        public int Complete()
        {
            return db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
