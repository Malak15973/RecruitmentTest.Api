﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos;
using System.Net;

namespace RecruitmentTest.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public SkillsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var data = await unitOfWork.Skills.GetAllAsync();

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
