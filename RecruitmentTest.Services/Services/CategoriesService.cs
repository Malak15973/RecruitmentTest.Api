using AutoMapper;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Services.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CategoriesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> GetAllCategories()
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            try
            {
                var data = await unitOfWork.Categories.GetAllAsync();
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
