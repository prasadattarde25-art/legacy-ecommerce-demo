using System.Collections.Generic;

namespace Ecommerce.Core.Common
{
    public class ServiceResult<T> : ServiceResult
    {
        public T Value { get; set; }

        public static ServiceResult<T> Ok(T value, string message = null)
        {
            return new ServiceResult<T> { Success = true, Value = value, Message = message };
        }

        public new static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T> { Success = false, Message = message };
        }
    }
}