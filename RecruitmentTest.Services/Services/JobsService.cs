using AutoMapper;
using Microsoft.AspNetCore.Http;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Jobs;
using RecruitmentTest.Domain.Helpers;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Services.Services
{
    public class JobsService : IJobsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public JobsService(IUnitOfWork unitOfWork,IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse> AddJob(AddJobDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                #region check if any request id is wrong 
                if (await unitOfWork.Categories.GetByIdAsync(model.CategoryId) == null)
                {
                    response.ErrorMessages.Add($"No Category With This Id {model.CategoryId}");
                    return response;
                }
                for (int i = 0; i < model.JobSkills.Count; i++)
                {
                    if (await unitOfWork.Skills.GetByIdAsync(model.JobSkills[i]) == null)
                    {
                        response.ErrorMessages.Add($"No Skill With This Id {model.JobSkills[i]}");
                        return response;
                    }
                }
                for (int i = 0; i < model.JobResponsabilities.Count; i++)
                {
                    if (await unitOfWork.Responsabilities.GetByIdAsync(model.JobResponsabilities[i]) == null)
                    {
                        response.ErrorMessages.Add($"No Responsability With This Id {model.JobResponsabilities[i]}");
                        return response;
                    }
                }
                #endregion

                await unitOfWork.Jobs.AddAsync(mapper.Map<Job>(model));
                unitOfWork.Complete();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }

        public async Task<ApiResponse> ApplyToJob(int jobId)
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
                    return response;
                }
                var userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid").Value;
                //check is job opened and validate that the user does not apply to this job before
                if (JobsHelper.IsOpenJob(job) && await unitOfWork.ApplicationUsersJobs.FindAsync(aj => aj.ApplicationUserId == userId && aj.JobId == jobId) == null)
                {
                    await unitOfWork.ApplicationUsersJobs.AddAsync(new ApplicationUserJob()
                    {
                        JobId = jobId,
                        ApplicationUserId = userId
                    });
                    unitOfWork.Complete();
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
                else
                {
                    response.ErrorMessages.Add("Job Is Closed");
                    return response;
                }
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }

        public async Task<ApiResponse> DeleteJob(int id)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var job = await unitOfWork.Jobs.GetByIdAsync(id);
                if (job == null)
                {
                    response.ErrorMessages.Add($"No Job With This Id {id}");
                    return response;
                }
                unitOfWork.Jobs.Delete(job);
                unitOfWork.Complete();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }

        public async Task<PaggingApiResponse> GetAllJobs(int page = 1, int pageSize = 3, string name = "")
        {
            var response = new PaggingApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var data = await unitOfWork.Jobs.GetAllAsync(filter: f => f.Name.Contains(name), skip: (page - 1) * pageSize, take: pageSize,
                    includeProperties: "JobResponsabilities.Responsability,JobSkills.Skill,Category,JobApplicationUsers");

                var totalCount = unitOfWork.Jobs.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.TotalCount = totalCount;
                response.TotalPages = totalPages;
                response.CurrentPage = page;
                response.PageSize = pageSize;
                response.Result = mapper.Map<IEnumerable<JobDto>>(data);
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }

        public async Task<ApiResponse> GetJob(int id)
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
                    return response;
                }
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = mapper.Map<JobDto>(job);
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }

        public async Task<PaggingApiResponse> GetNotAppliedJobs(int page = 1, int pageSize = 3, string name = "")
        {
            var response = new PaggingApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid").Value;

                var data = await unitOfWork.Jobs.GetAllAsync(filter: f => f.Name.Contains(name) && !f.JobApplicationUsers.Any(ja => ja.ApplicationUserId == userId), skip: (page - 1) * pageSize, take: pageSize,
                    includeProperties: "JobResponsabilities.Responsability,JobSkills.Skill,Category,JobApplicationUsers");

                var totalCount = unitOfWork.Jobs.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.TotalCount = totalCount;
                response.TotalPages = totalPages;
                response.CurrentPage = page;
                response.PageSize = pageSize;
                response.Result = mapper.Map<IEnumerable<JobDto>>(data);
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
           }

        public async Task<ApiResponse> UpdateJob(int id, UpdateJobDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var job = await unitOfWork.Jobs.FindAsync(j => j.Id == id, new[] { "JobResponsabilities", "JobSkills" });
                if (job == null)
                {
                    response.ErrorMessages.Add($"No Job With This Id {id}");
                    return response;
                }
                #region check if any request id is wrong 
                if (await unitOfWork.Categories.GetByIdAsync(model.CategoryId) == null)
                {
                    response.ErrorMessages.Add($"No Category With This Id {model.CategoryId}");
                    return response;
                }
                for (int i = 0; i < model.JobSkills.Count; i++)
                {
                    if (await unitOfWork.Skills.GetByIdAsync(model.JobSkills[i]) == null)
                    {
                        response.ErrorMessages.Add($"No Skill With This Id {model.JobSkills[i]}");
                        return response;
                    }
                }
                for (int i = 0; i < model.JobResponsabilities.Count; i++)
                {
                    if (await unitOfWork.Responsabilities.GetByIdAsync(model.JobResponsabilities[i]) == null)
                    {
                        response.ErrorMessages.Add($"No Responsability With This Id {model.JobResponsabilities[i]}");
                        return response;
                    }
                }
                #endregion


                unitOfWork.JobsResponsabilities.DeleteRange(job.JobResponsabilities);
                unitOfWork.JobsSkills.DeleteRange(job.JobSkills);


                unitOfWork.Jobs.Update(mapper.Map<Job>(model));
                unitOfWork.Complete();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return response;
             
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }
    }
}
