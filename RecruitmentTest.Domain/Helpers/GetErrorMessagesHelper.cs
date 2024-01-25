using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Helpers
{
    public static class GetErrorMessagesHelper
    {
        public static List<string> GetErrorMessages(ModelStateDictionary modelState)
        {
            List<string> errorMessages = new List<string>();

            foreach (var modelStateEntry in modelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    errorMessages.Add(error.ErrorMessage);
                }
            }

            return errorMessages;
        }
    }
}
