using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecruitmentTest.Domain;
using RecruitmentTest.Domain.Dtos.Account;
using RecruitmentTest.Domain.Models;
using RecruitmentTest.Domain.Settings;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
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
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "Email Or Password Is Invalid!");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }
                    var result = await signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (result.Succeeded)
                    {
                        var jwtSecurityToken = await CreateJwtToken(user);
                        var roles = await userManager.GetRolesAsync(user);
                        return Ok(new { data = new { Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), ExpiresOn = jwtSecurityToken.ValidTo, Role = roles.FirstOrDefault() } });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email Or Password Is Invalid!");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }
                }
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await userManager.FindByEmailAsync(model.Email) != null)
                    {
                        ModelState.AddModelError(string.Empty, $"User Is Already Exist");
                        return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
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
                        return Ok();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, $"{error.Description}");
                            return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                        }
                        return BadRequest(new { errors = ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
                    }
                }
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, $"{e}");
                return BadRequest(new { errors = ModelState.Select(m => m.Value!.Errors.Select(e => e.ErrorMessage)).Where(p => p.Any()) });
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