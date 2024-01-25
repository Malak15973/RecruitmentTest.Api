using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos.Jobs;
using RecruitmentTest.Domain.Models;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecruitmentTest.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public JobsController(IMapper mapper,IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddJob([FromBody] AddJobDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await unitOfWork.Categories.GetByIdAsync(model.CategoryId)==null)
                    {
                        ModelState.AddModelError(string.Empty, $"No Category With This Id {model.CategoryId}");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }

                    Job job = new()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        CategoryId = model.CategoryId,
                        ValidTo = model.ValidTo,
                        ValidFrom = model.ValidFrom,
                        MaxApplicants = model.MaxApplicants,
                        JobResponsabilities = model.JobResponsabilities.Select(j => new JobResponsability() { ResponsabilityId = j }).ToList(),
                        JobSkills = model.JobSkills.Select(j => new JobSkill() { SkillId = j }).ToList()
                    };
                    await unitOfWork.Jobs.AddAsync(job);
                    unitOfWork.Complete();
                    return Ok();
                }
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                var job = await unitOfWork.Jobs.GetByIdAsync(id);
                if (job==null)
                {
                    ModelState.AddModelError(string.Empty, $"No Job With This Id {id}");
                    return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                }
                unitOfWork.Jobs.Delete(job);
                unitOfWork.Complete();
                return Ok();
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] AddJobDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var job = await unitOfWork.Jobs.FindAsync(j=>j.Id==id, new[] { "JobResponsabilities", "JobSkills" });
                    if (job == null)
                    {
                        ModelState.AddModelError(string.Empty, $"No Job With This Id {id}");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }

                    if (await unitOfWork.Categories.GetByIdAsync(model.CategoryId) == null)
                    {
                        ModelState.AddModelError(string.Empty, $"No Category With This Id {model.CategoryId}");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }


                    job.Name = model.Name;
                    job.Description = model.Description ;
                    job.CategoryId = model.CategoryId;
                    job.MaxApplicants = model.MaxApplicants;
                    job.ValidFrom = model.ValidFrom;
                    job.ValidTo= model.ValidTo;

                    unitOfWork.JobsResponsabilities.DeleteRange(job.JobResponsabilities);
                    unitOfWork.JobsSkills.DeleteRange(job.JobSkills);


                    job.JobResponsabilities = model.JobResponsabilities.Select(j => new JobResponsability() { ResponsabilityId = j }).ToList();
                    job.JobSkills = model.JobSkills.Select(j => new JobSkill() { SkillId = j }).ToList();


                    unitOfWork.Jobs.Update(job);
                    unitOfWork.Complete();
                    return Ok();
                }
                return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            try
            {
                var job = await unitOfWork.Jobs.FindAsync(p => p.Id == id,
                    new[] {
                        "JobApplicationUsers","JobResponsabilities.Responsability", "JobSkills.Skill", "Category",
                    });
                if (job == null)
                {
                    ModelState.AddModelError(string.Empty, $"No Job With Id {id}");
                    return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                }
                return Ok(mapper.Map<JobDto>(job));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string name = "")
        {
            try
            {
                var data = await unitOfWork.Jobs.GetAllAsync(filter: f=>f.Name.Contains(name), skip: (page - 1) * pageSize, take: pageSize,
                    includeProperties: "JobResponsabilities.Responsability,JobSkills.Skill,Category,JobApplicationUsers");

                var totalCount = unitOfWork.Jobs.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Jobs = mapper.Map<IEnumerable<JobDto>>(data)
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("{jobId}")]
        public async Task<IActionResult> ApplyToJob(int jobId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var job = await unitOfWork.Jobs.FindAsync(p => p.Id == jobId,
                    new[] {
                        "JobApplicationUsers",
                    });
                    if (await unitOfWork.Jobs.GetByIdAsync(jobId) == null)
                    {
                        ModelState.AddModelError(string.Empty, $"No Job With This Id {jobId}");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }

                    bool isOpen = (job.JobApplicationUsers.Count < job.MaxApplicants)&&(DateTime.Now>=job.ValidFrom&& DateTime.Now <= job.ValidTo);
                    var userId = User.Claims.Where(a => a.Type == "uid").FirstOrDefault().Value;

                    if (isOpen) {
                        await unitOfWork.ApplicationUsersJobs.AddAsync(new ApplicationUserJob()
                        {
                            JobId = jobId,
                            ApplicationUserId = userId
                        });
                        unitOfWork.Complete();
                        return Ok();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Jop Is Closed");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }
                }
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

    }
}
