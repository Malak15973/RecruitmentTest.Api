using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos.Jobs;
using RecruitmentTest.Domain.Dtos;
using System.Net;

namespace RecruitmentTest.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public CategoriesController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var data = await unitOfWork.Categories.GetAllAsync();
                response.IsSuccess = true;
                response.Result = mapper.Map<IEnumerable<SelectListDto>>(data);
                return Ok(response);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }
    }
}
