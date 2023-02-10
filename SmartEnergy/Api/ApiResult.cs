using System.Net;

namespace SmartEnergy.Api
{
    public class ApiResult<T>
    {
        public ApiResult(T value, HttpStatusCode statusCode) 
        {
            Value = value;
            StatusCode = statusCode;
        }

        public ApiResult(string message, HttpStatusCode statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }

        public T Value { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }
        public bool Succes => Value != null;
    }
}
