using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Contract;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                            .Where(x => x.Value?.Errors?.Any() == true)
                            .SelectMany(x => x.Value?.Errors?.Where(e => e != null)
                            .Select(e => new ValidationFailure(x.Key, e.ErrorMessage)) ?? [])
                            .ToList();

            throw new ValidationException(errors);
        }
    }
}
