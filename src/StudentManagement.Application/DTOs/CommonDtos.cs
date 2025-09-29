namespace StudentManagement.Application.DTOs;

// Common response patterns
public record PagedResultDto<T>
{
    public ICollection<T> Items { get; init; } = new List<T>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}

public record ApiResponseDto<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public ICollection<string> Errors { get; init; } = new List<string>();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponseDto<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully"
        };
    }

    public static ApiResponseDto<T> ErrorResult(string error, T? data = default)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Data = data,
            Errors = new List<string> { error }
        };
    }

    public static ApiResponseDto<T> ErrorResult(ICollection<string> errors, T? data = default)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Data = data,
            Errors = errors
        };
    }
}

// Base pagination request
public abstract record PagedRequestDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

// Common validation result
public record ValidationResultDto
{
    public bool IsValid { get; init; }
    public ICollection<string> Errors { get; init; } = new List<string>();
}