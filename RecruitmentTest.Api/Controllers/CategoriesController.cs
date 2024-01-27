using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Services.IServices;

namespace RecruitmentTest.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesService categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            this.categoriesService = categoriesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await categoriesService.GetAllCategories();
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
    }
}
