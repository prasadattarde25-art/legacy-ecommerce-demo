using System.Collections.Generic;

namespace Ecommerce.Core.Common
{
    public class ServiceResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public IList<string> Errors { get; set; }

        public ServiceResult()
        {
            Errors = new List<string>();
        }

        public static ServiceResult Ok(string message = null)
        {
            return new ServiceResult { Success = true, Message = message };
        }

        public static ServiceResult Fail(string message)
        {
            return new ServiceResult { Success = false, Message = message };
        }
    }
}