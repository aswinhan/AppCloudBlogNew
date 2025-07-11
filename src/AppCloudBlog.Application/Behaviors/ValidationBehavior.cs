namespace AppCloudBlog.Application.Behaviors;

// This behavior intercepts MediatR requests (Commands/Queries)
// and runs all registered Fluent Validators against them.
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                // Throw a custom validation exception if any validation fails
                throw new Exceptions.ValidationException(failures);
            }
        }
        return await next(cancellationToken); // Proceed to the next behavior or the handler
    }
}