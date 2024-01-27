using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Jobs;
using RecruitmentTest.Domain.Helpers;
using RecruitmentTest.Services.IServices;
using System.Net;

namespace RecruitmentTest.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobsService jobsService;

        public JobsController(IJobsService jobsService)
        {
            this.jobsService = jobsService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddJob([FromBody] AddJobDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            if (!ModelState.IsValid)
            {
                response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                return BadRequest(response);
            }
            else
            {
                response =await jobsService.AddJob(model);
                if (response.IsSuccess) return Ok(response);
                return BadRequest(response);
            }
        }
            

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var response = await jobsService.DeleteJob(id);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            if (!ModelState.IsValid)
            {
                response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                return BadRequest(response);
            }
            else
            {
                response = await jobsService.UpdateJob(id,model);
                if (response.IsSuccess) return Ok(response);
                return BadRequest(response);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var response = await jobsService.GetJob(id);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string name = "")
        {
            var response = await jobsService.GetAllJobs(page,pageSize,name);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
        [Authorize(Roles = "User")]
        [HttpGet]

        public async Task<IActionResult> GetNotAppliedJobs([FromQuery] int page = 1, [FromQuery] int pageSize = 3, [FromQuery] string name = "")
        {
            var userId = User.Claims.Where(a => a.Type == "uid").FirstOrDefault().Value;

            var response = await jobsService.GetNotAppliedJobs(userId,page, pageSize, name);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }


        [Authorize(Roles = "User")]
        [HttpPost("{jobId}")]
        public async Task<IActionResult> ApplyToJob(int jobId)
        {
           var userId = User.Claims.Where(a => a.Type == "uid").FirstOrDefault().Value;
            var response = await jobsService.ApplyToJob(jobId,userId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

    }
}
