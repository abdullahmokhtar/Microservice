namespace Microservices.Services.ProductAPI.Models.Dto;

public class ResultDto<T>
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public T? Data { get; private set; }

    public static ResultDto<T> SuccessResult(T data, string message = "Operation completed successfully.")
    {
        return new ResultDto<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResultDto<T> FailureResult(string message)
    {
        return new ResultDto<T>
        {
            Success = false,
            Message = message,
        };
    }
}
