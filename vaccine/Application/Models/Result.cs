namespace vaccine.Application.Models;

public class Result<T> where T : class
{
    public bool Success { get; set; }
    public T? Data { get; set; } = null;
    public string? Error { get; set; }

    public static Result<T> Failure(string message)
    {
        return new Result<T>()
        {
            Success = false,
            Error = message,
        };
    }

    public static Result<T> Ok(T data)
    {
        return new Result<T>()
        {
            Success = true,
            Data = data,
        };
    }
}