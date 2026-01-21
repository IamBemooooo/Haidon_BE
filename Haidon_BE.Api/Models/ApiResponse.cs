namespace Haidon_BE.Api.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public object? Errors { get; set; }

    public ApiResponse(bool success, string? message = null, T? data = default, object? errors = null)
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors;
    }

    public static ApiResponse<T> SuccessResponse(T? data = default, string? message = null)
        => new(true, message, data);
    public static ApiResponse<T> FailResponse(string? message = null, object? errors = null)
        => new(false, message, default, errors);
}
