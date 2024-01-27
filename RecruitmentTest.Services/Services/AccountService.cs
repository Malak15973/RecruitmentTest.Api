using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Account;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Domain.Settings;
using RecruitmentTest.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using RecruitmentTest.Domain.Helpers;
using System.Net;

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
        public async Task<ApiResponse> Login(LoginDto model)
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

        public async Task<ApiResponse> Register(RegisterDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                if (await userManager.FindByEmailAsync(model.Email) != null)
                {
                    response.ErrorMessages.Add("User Is Already Exist");
                    return response;
                }
                ApplicationUser user = new()
                {
                    UserName = model.Email,
                    Name = model.Name,
                    Email = model.Email,
                    PhoneNumber = model.MobileNumber,
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
                else
                {
                    response.ErrorMessages.AddRange(result.Errors.Select(e => e.Description));
                }
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
