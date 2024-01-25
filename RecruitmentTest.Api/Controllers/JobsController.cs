using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Jobs;
using RecruitmentTest.Domain.Helpers;
using RecruitmentTest.Domain.Models;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
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
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                if (!ModelState.IsValid)
                {
                    response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                    return BadRequest(response);
                }
                else
                {
                    if (await unitOfWork.Categories.GetByIdAsync(model.CategoryId) == null)
                    {
                        response.ErrorMessages.Add($"No Category With This Id {model.CategoryId}");
                        return BadRequest(response);
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
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var job = await unitOfWork.Jobs.GetByIdAsync(id);
                if (job==null)
                {
                    response.ErrorMessages.Add($"No Job With This Id {id}");
                    return BadRequest(response);
                }
                unitOfWork.Jobs.Delete(job);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Ok(response);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] AddJobDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                if (!ModelState.IsValid)
                {
                    response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                    return BadRequest(response);
                }
                else
                {
                    var job = await unitOfWork.Jobs.FindAsync(j => j.Id == id, new[] { "JobResponsabilities", "JobSkills" });
                    if (job == null)
                    {
                        response.ErrorMessages.Add($"No Job With This Id {id}");
                        return BadRequest(response);
                    }

                    if (await unitOfWork.Categories.GetByIdAsync(model.CategoryId) == null)
                    {
                        response.ErrorMessages.Add($"No Category With This Id {model.CategoryId}");
                        return BadRequest(response);
                    }


                    job.Name = model.Name;
                    job.Description = model.Description;
                    job.CategoryId = model.CategoryId;
                    job.MaxApplicants = model.MaxApplicants;
                    job.ValidFrom = model.ValidFrom;
                    job.ValidTo = model.ValidTo;

                    unitOfWork.JobsResponsabilities.DeleteRange(job.JobResponsabilities);
                    unitOfWork.JobsSkills.DeleteRange(job.JobSkills);


                    job.JobResponsabilities = model.JobResponsabilities.Select(j => new JobResponsability() { ResponsabilityId = j }).ToList();
                    job.JobSkills = model.JobSkills.Select(j => new JobSkill() { SkillId = j }).ToList();


                    unitOfWork.Jobs.Update(job);
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var job = await unitOfWork.Jobs.FindAsync(p => p.Id == id,
                    new[] {
                        "JobApplicationUsers","JobResponsabilities.Responsability", "JobSkills.Skill", "Category",
                    });
                if (job == null)
                {
                    response.ErrorMessages.Add($"No Job With This Id {id}");
                    return BadRequest(response);
                }
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = mapper.Map<JobDto>(job);
                return Ok(response);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string name = "")
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var data = await unitOfWork.Jobs.GetAllAsync(filter: f=>f.Name.Contains(name), skip: (page - 1) * pageSize, take: pageSize,
                    includeProperties: "JobResponsabilities.Responsability,JobSkills.Skill,Category,JobApplicationUsers");

                var totalCount = unitOfWork.Jobs.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = new
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.OK,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Result = mapper.Map<IEnumerable<JobDto>>(data)
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }
        [Authorize(Roles = "User")]
        [HttpGet]

        public async Task<IActionResult> GetNotAppliedJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string name = "")
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var userId = User.Claims.Where(a => a.Type == "uid").FirstOrDefault().Value;

                var data = await unitOfWork.Jobs.GetAllAsync(filter: f => f.Name.Contains(name)&&!f.JobApplicationUsers.Any(ja=>ja.ApplicationUserId==userId), skip: (page - 1) * pageSize, take: pageSize,
                    includeProperties: "JobResponsabilities.Responsability,JobSkills.Skill,Category,JobApplicationUsers");

                var totalCount = unitOfWork.Jobs.Count(filter: f => f.Name.Contains(name) && !f.JobApplicationUsers.Any(ja => ja.ApplicationUserId == userId), includeProperties: "JobApplicationUsers");
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var result = new
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.OK,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Result = mapper.Map<IEnumerable<JobDto>>(data)
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }


        [Authorize(Roles = "User")]
        [HttpPost("{jobId}")]
        public async Task<IActionResult> ApplyToJob(int jobId)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var job = await unitOfWork.Jobs.FindAsync(p => p.Id == jobId,
                new[] {
                    "JobApplicationUsers",
                });
                if (await unitOfWork.Jobs.GetByIdAsync(jobId) == null)
                {
                    response.ErrorMessages.Add($"No Job With This Id {jobId}");
                    return BadRequest(response);
                }

                var userId = User.Claims.Where(a => a.Type == "uid").FirstOrDefault().Value;

                if (JobsHelper.IsOpenJob(job)&&await unitOfWork.ApplicationUsersJobs.FindAsync(aj=>aj.ApplicationUserId==userId&&aj.JobId==jobId)==null) {
                    await unitOfWork.ApplicationUsersJobs.AddAsync(new ApplicationUserJob()
                    {
                        JobId = jobId,
                        ApplicationUserId = userId
                    });
                    unitOfWork.Complete();
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.ErrorMessages.Add("Job Is Closed");
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }

    }
}
