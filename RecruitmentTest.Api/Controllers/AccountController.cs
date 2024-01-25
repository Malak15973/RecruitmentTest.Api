using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos;
using RecruitmentTest.Domain.Dtos.Account;
using RecruitmentTest.Domain.Helpers;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Domain.Settings;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RecruitmentTest.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTSettings jwt;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(IOptions<JWTSettings> jwt,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.jwt = jwt.Value;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                if(!ModelState.IsValid)
                {
                    response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                    return BadRequest(response);
                }
                else
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
                            return Ok(response);
                        }

                    }
                }
                response.ErrorMessages.Add("Email Or Password Is Invalid!");
                return BadRequest(response);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var response = new ApiResponse() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                if (!ModelState.IsValid)
                {
                    response.ErrorMessages.AddRange(GetErrorMessagesHelper.GetErrorMessages(ModelState));
                    return BadRequest(response);
                }
                else
                {
                    if (await userManager.FindByEmailAsync(model.Email) != null)
                    {
                        response.ErrorMessages.Add("User Is Already Exist");
                        return BadRequest(response);
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
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessages.AddRange(result.Errors.Select(e => e.Description));
                    }
                }
                return BadRequest(response);
            }
            catch (Exception e)
            {
                response.ErrorMessages.Add(e.ToString());
                return BadRequest(response);
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