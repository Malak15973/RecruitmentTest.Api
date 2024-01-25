using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Account;
using RecruitmentTest.Domain.Helpers;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Domain.Settings;
using RecruitmentTest.Services.IServices;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly JWTSettings jwt;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountService(IOptions<JWTSettings> jwt, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.jwt = jwt.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public async Task<ApiResponse> LoginUserAsync(LoginDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (result.Succeeded)
                    {
                        var jwtSecurityToken = await CreateJwtToken(user);
                        var roles = await userManager.GetRolesAsync(user);
                        response.Result = new { Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), ExpiresOn = jwtSecurityToken.ValidTo, Role = roles.FirstOrDefault() };
                        response.IsSuccess = true;
                        response.StatusCode = HttpStatusCode.OK;
                        return response;
                    }

                }
                
                response.ErrorMessages.Add("Email Or Password Is Invalid!");
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return response;
            }
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
                roleClaims.Add(new Claim("roles", roles[i]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
            }.Union(roleClaims);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
