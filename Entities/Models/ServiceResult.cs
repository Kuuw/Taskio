namespace Entities.Models
{
    public class ServiceResult<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public static ServiceResult<T> Ok(T data)
        {
            return new ServiceResult<T> { Data = data, Success = true, StatusCode = 200 };
        }

        public static ServiceResult<T> BadRequest(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 400 };
        }

        public static ServiceResult<T> NotFound(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 404 };
        }

        public static ServiceResult<T> InternalServerError(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 500 };
        }

        public static ServiceResult<T> Unauthorized(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 401 };
        }

        public static ServiceResult<T> Forbidden(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 403 };
        }

        public static ServiceResult<T> Conflict(string errorMessage)
        {
            return new ServiceResult<T> { Success = false, ErrorMessage = errorMessage, StatusCode = 409 };
        }

        public static ServiceResult<T> NoContent()
        {
            return new ServiceResult<T> { Success = true, StatusCode = 204 };
        }

        public static ServiceResult<T> Created(T data)
        {
            return new ServiceResult<T> { Data = data, Success = true, StatusCode = 201 };
        }

        public static ServiceResult<T> Created()
        {
            return new ServiceResult<T> { Success = true, StatusCode = 201 };
        }

        public static ServiceResult<T> Accepted()
        {
            return new ServiceResult<T> { Success = true, StatusCode = 202 };
        }
    }
}