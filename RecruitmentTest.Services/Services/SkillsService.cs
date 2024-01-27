using AutoMapper;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RecruitmentTest.Services.IServices;

namespace RecruitmentTest.Services.Services
{
    public class SkillsService: ISkillsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SkillsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> GetAllSkills()
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var data = await unitOfWork.Skills.GetAllAsync();
                response.IsSuccess = true;
                response.Result = mapper.Map<IEnumerable<SelectListDto>>(data);
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
