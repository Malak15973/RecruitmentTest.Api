using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Services.IServices
{
    public interface IAccountService
    {
        Task<ApiResponse> Login(LoginDto model);
        Task<ApiResponse> Register(RegisterDto model);
        Task<ApiResponse> RefreshToken(TokenDto model);
    }
}
