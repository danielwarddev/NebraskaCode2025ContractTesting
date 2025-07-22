using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductApi.ContractTests;

public class UnknownQueryParamValidatorFilter : IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var validParameters = new HashSet<string>(
            context.ActionDescriptor.Parameters.Select(x => x.Name),
            StringComparer.OrdinalIgnoreCase
        );
        var unknownParameters = new Dictionary<string, string[]>();

        foreach (var providedParameter in context.HttpContext.Request.Query)
        {
            if (!validParameters.Contains(providedParameter.Key))
            {
                unknownParameters.Add(providedParameter.Key,
                [$"Query string \"{providedParameter.Key}\" does not bind to any parameter. " +
                 $"Valid parameter names for endpoint {context.HttpContext.Request.Path} " +
                 $"are: {string.Join(", ", validParameters)}."]);
            }
        }

        if (unknownParameters.Any())
        {
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(unknownParameters));
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}