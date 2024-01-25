using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecruitmentTest.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.DataAccess
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUserJob> ApplicationUsersJobs { get; set; } 
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Job> Jobs { get; set; } 
        public DbSet<JobResponsability> JobsResponsabilities { get; set; } 
        public DbSet<JobSkill> JobsSkills { get; set; }
        public DbSet<Responsability> Responsabilities { get; set; } 
        public DbSet<Skill> Skills { get; set; } 


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            #region Seed User Role
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895722", Name = "User", ConcurrencyStamp = "1", NormalizedName = "USER" }
            );
            #endregion

            #region Seed Admin Account
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "ADMIN" }
            );
            ApplicationUser admin = new()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                NormalizedUserName = "Admin",
                Email = "Admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                LockoutEnabled = false,
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, "Admin1234")
            };
            builder.Entity<ApplicationUser>().HasData(admin);
            builder.Entity<IdentityUserRole<string>>().HasData(
               new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" }
            );
            #endregion

            #region Seed Categories
            builder.Entity<Category>().HasData(
                new Category() { Id = 1, Name = "Cat1"},
                new Category() { Id = 2, Name = "Cat2" },
                new Category() { Id = 3, Name = "Cat3" }
            );
            #endregion

            #region Seed Skills
            builder.Entity<Skill>().HasData(
                new Skill() { Id = 1, Name = "Skill1" },
                new Skill() { Id = 2, Name = "Skill2" },
                new Skill() { Id = 3, Name = "Skill3" }
            );
            #endregion

            #region Seed Responsabilities
            builder.Entity<Responsability>().HasData(
                new Responsability() { Id = 1, Name = "Resp1" },
                new Responsability() { Id = 2, Name = "Resp2" },
                new Responsability() { Id = 3, Name = "Resp3" }
            );
            #endregion

            #region Many To Many Job and Responsability
            builder.Entity<JobResponsability>()
                 .HasKey(qd => new { qd.JobId, qd.ResponsabilityId });
            builder.Entity<JobResponsability>()
                .HasOne(qd => qd.Job)
                .WithMany(q => q.JobResponsabilities)
                .HasForeignKey(qd => qd.JobId);
            builder.Entity<JobResponsability>()
                .HasOne(tv => tv.Responsability)
                .WithMany(v => v.JobsResponsabilities)
                .HasForeignKey(tv => tv.ResponsabilityId);
            #endregion

            #region Many To Many Job and Skill
            builder.Entity<JobSkill>()
                 .HasKey(qd => new { qd.JobId, qd.SkillId });
            builder.Entity<JobSkill>()
                .HasOne(qd => qd.Job)
                .WithMany(q => q.JobSkills)
                .HasForeignKey(qd => qd.JobId);
            builder.Entity<JobSkill>()
                .HasOne(tv => tv.Skill)
                .WithMany(v => v.JobsSkills)
                .HasForeignKey(tv => tv.SkillId);
            #endregion

            #region Many To Many User and Job
            builder.Entity<ApplicationUserJob>()
                 .HasKey(qd => new { qd.ApplicationUserId, qd.JobId });
            builder.Entity<ApplicationUserJob>()
                .HasOne(qd => qd.ApplicationUser)
                .WithMany(q => q.ApplicationUserJobs)
                .HasForeignKey(qd => qd.ApplicationUserId);
            builder.Entity<ApplicationUserJob>()
                .HasOne(tv => tv.Job)
                .WithMany(v => v.JobApplicationUsers)
                .HasForeignKey(tv => tv.JobId);
            #endregion
        }
    }
}
