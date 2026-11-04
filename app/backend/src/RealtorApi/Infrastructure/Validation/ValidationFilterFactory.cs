using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace RealtorApi.Infrastructure.Validation;

public static class ValidationFilterFactory
{
    public static EndpointFilterDelegate Create(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        var parameters = context.MethodInfo.GetParameters();
        var serviceProviderIsService = context.ApplicationServices.GetService<IServiceProviderIsService>();

        var candidates = new List<(int Index, Type ParameterType, Type ValidatorType)>();

        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var parameterType = parameter.ParameterType;

            if (parameterType == typeof(HttpContext) ||
                parameterType == typeof(CancellationToken) ||
                parameterType == typeof(HttpRequest) ||
                parameterType == typeof(HttpResponse) ||
                parameterType == typeof(ClaimsPrincipal))
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);
            var hasValidator = serviceProviderIsService?.IsService(validatorType)
                ?? context.ApplicationServices.GetService(validatorType) is not null;

            if (hasValidator)
            {
                candidates.Add((index, parameterType, validatorType));
            }
        }

        if (candidates.Count == 0)
        {
            return next;
        }

        return async invocationContext =>
        {
            Dictionary<string, string[]>? validationErrors = null;

            foreach (var candidate in candidates)
            {
                var argument = invocationContext.Arguments[candidate.Index];
                if (argument is null)
                {
                    continue;
                }

                var validator = invocationContext.HttpContext.RequestServices.GetService(candidate.ValidatorType) as IValidator;
                if (validator is null)
                {
                    continue;
                }

                var result = await validator.ValidateAsync(
                    new ValidationContext<object>(argument),
                    invocationContext.HttpContext.RequestAborted);

                if (result.IsValid)
                {
                    continue;
                }

                validationErrors ??= new Dictionary<string, string[]>(StringComparer.Ordinal);

                foreach (var grouping in result.Errors.GroupBy(e => e.PropertyName))
                {
                    validationErrors[grouping.Key] = grouping
                        .Select(e => e.ErrorMessage)
                        .Distinct(StringComparer.Ordinal)
                        .ToArray();
                }
            }

            if (validationErrors is not null)
            {
                var details = new ValidationProblemDetails(validationErrors)
                {
                    Status = StatusCodes.Status400BadRequest
                };

                return TypedResults.BadRequest(details);
            }

            return await next(invocationContext);
        };
    }
}
