using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecruitmentTest.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Filters
{
    public class ModelStateValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // Handle validation errors here
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0).SelectMany(e => e.Value.Errors.Select(e => e.ErrorMessage)).
                ToList();
                ApiResponse response = new ()
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    ErrorMessages = errors
                };

            context.Result = new BadRequestObjectResult(response);
        }
    }
}

}
