using RecruitmentTest.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Services.IServices
{
    public interface IResponsabilitiesService
    {
        Task<ApiResponse> GetAllResponsabilities();
    }
}
