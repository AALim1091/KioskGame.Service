namespace KioskGame.Service;

public interface IRequestResult<T>
    {
        bool Success { get; }
        T? Data { get; }
        string? Error { get; }
        int? StatusCode { get; }
    }

    public class RequestResult<T> : IRequestResult<T>
    {
        public bool Success { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }
        public int? StatusCode { get; private set; }

        private RequestResult() { }

        public static RequestResult<T> Ok(T data) => new() { Success = true, Data = data, StatusCode = 200 };
        public static RequestResult<T> Created(T data) => new() { Success = true, Data = data, StatusCode = 201 };

        public static RequestResult<T> NoContent() => new() { Success = true, Data = default, StatusCode = 204 };
        public static RequestResult<T> NotFound(string? message = null) => new() { Success = false, Error = message ?? "Not found", StatusCode = 404 };
        public static RequestResult<T> BadRequest(string? message = null) => new() { Success = false, Error = message ?? "Bad request", StatusCode = 400 };
        public static RequestResult<T> Failure(string? message = null, int statusCode = 500) => new() { Success = false, Error = message ?? "An unexpected error occurred", StatusCode = statusCode };
    }

