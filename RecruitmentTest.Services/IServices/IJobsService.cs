using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Services.IServices
{
    public interface IJobsService
    {
        Task<ApiResponse> AddJob(AddJobDto model);
        Task<ApiResponse> DeleteJob(int id);
        Task<ApiResponse> UpdateJob(int id, UpdateJobDto model);
        Task<ApiResponse> GetJob(int id);
        Task<PaggingApiResponse> GetAllJobs(int page = 1,int pageSize = 3,string name = "");
        Task<PaggingApiResponse> GetNotAppliedJobs(int page = 1, int pageSize = 3, string name = "");
        Task<ApiResponse> ApplyToJob(int jobId);
    }
}
