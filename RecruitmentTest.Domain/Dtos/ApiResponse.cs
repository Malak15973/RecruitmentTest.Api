using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecruitmentTest.Domain.Dtos
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
