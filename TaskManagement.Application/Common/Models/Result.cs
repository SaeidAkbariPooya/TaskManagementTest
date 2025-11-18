using FluentValidation;
using MediatR;

namespace TaskManagement.Application.Common.Models
{
    public class Result
    {
        public bool Succeeded { get; }
        public string[] Errors { get; }

        protected Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public static Result Success() => new(true, Array.Empty<string>());
        public static Result Failure(IEnumerable<string> errors) => new(false, errors);
        public static Result Failure(string error) => new(false, new[] { error });
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool succeeded, T? data, IEnumerable<string> errors) : base(succeeded, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, Array.Empty<string>());
        public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
        public new static Result<T> Failure(string error) => new(false, default, new[] { error });
    }

    // TaskManager.Application/Common/Behaviors/ValidationBehavior.cs
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
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Any())
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
