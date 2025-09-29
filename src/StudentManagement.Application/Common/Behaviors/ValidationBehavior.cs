using FluentValidation;
using MediatR;
using StudentManagement.Application.DTOs;

namespace StudentManagement.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                return CreateValidationErrorResponse<TResponse>(failures);
            }
        }

        return await next();
    }

    private static T CreateValidationErrorResponse<T>(List<FluentValidation.Results.ValidationFailure> failures)
    {
        var errors = failures.Select(f => f.ErrorMessage).ToList();

        // Check if T is ApiResponseDto<T>
        var responseType = typeof(T);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ApiResponseDto<>))
        {
            var dataType = responseType.GetGenericArguments()[0];
            var method = typeof(ApiResponseDto<>)
                .MakeGenericType(dataType)
                .GetMethod("ErrorResult", new[] { typeof(ICollection<string>), dataType });

            var result = method?.Invoke(null, new object?[] { errors, default });
            return (T)result!;
        }

        // Fallback for other response types
        throw new ValidationException("Validation failed", failures);
    }
}