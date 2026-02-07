namespace ZuluVault.Domain.Common;

/// <summary>
/// Generic result wrapper for operation outcomes
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result<T> Success(T data, string message = "Operation completed successfully")
        => new Result<T> { IsSuccess = true, Data = data, Message = message };

    public static Result<T> Failure(string message, List<string>? errors = null)
        => new Result<T> { IsSuccess = false, Data = default, Message = message, Errors = errors ?? new() };

    public static Result<T> Failure(List<string> errors)
        => new Result<T> { IsSuccess = false, Data = default, Message = "Validation failed", Errors = errors };
}

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result Success(string message = "Operation completed successfully")
        => new Result { IsSuccess = true, Message = message };

    public static Result Failure(string message, List<string>? errors = null)
        => new Result { IsSuccess = false, Message = message, Errors = errors ?? new() };

    public static Result Failure(List<string> errors)
        => new Result { IsSuccess = false, Message = "Validation failed", Errors = errors };
}
