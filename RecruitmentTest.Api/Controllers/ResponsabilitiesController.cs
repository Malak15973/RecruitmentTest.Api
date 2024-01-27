using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Services.IServices;

namespace RecruitmentTest.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ResponsabilitiesController : ControllerBase
    {
        private readonly IResponsabilitiesService responsabilitiesService;

        public ResponsabilitiesController(IResponsabilitiesService responsabilitiesService)
        {
            this.responsabilitiesService = responsabilitiesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllResponsabilities()
        {
            var response = await responsabilitiesService.GetAllResponsabilities();
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
    }
}
