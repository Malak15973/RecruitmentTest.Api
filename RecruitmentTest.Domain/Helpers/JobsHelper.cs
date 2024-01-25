using Microsoft.AspNetCore.Mvc.ModelBinding;
using RecruitmentTest.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Helpers
{
    public static class JobsHelper
    {
        public static bool IsOpenJob(Job job)
        {
            return (job.JobApplicationUsers.Count < job.MaxApplicants) && (DateTime.Now >= job.ValidFrom && DateTime.Now <= job.ValidTo);
        }
    }
}
