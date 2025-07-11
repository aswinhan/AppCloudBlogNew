// Add a using alias to resolve the ambiguity
using FluentValidationValidationFailure = FluentValidation.Results.ValidationFailure;

namespace AppCloudBlog.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        // Use the alias here
        public ValidationException(IEnumerable<FluentValidationValidationFailure> failures)
            : this()
        {
            var propertyNames = failures
               .Select(e => e.PropertyName)
               .Distinct();

            foreach (var propertyName in propertyNames)
            {
                Errors.Add(propertyName, failures
                   .Where(e => e.PropertyName == propertyName)
                   .Select(e => e.ErrorMessage)
                   .ToArray());
            }
        }

        public IDictionary<string, string[]> Errors { get; }
    }
}