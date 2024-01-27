using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Services.IServices;

namespace RecruitmentTest.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService skillsService;

        public SkillsController(ISkillsService skillsService)
        {
            this.skillsService = skillsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var response = await skillsService.GetAllSkills();
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
    }
}
