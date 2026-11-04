using FluentValidation;
using Microsoft.AspNetCore.Http;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApiTests.Infrastructure.Validation;

public sealed record ValidatedRequest(string Name);

public sealed class ValidatedRequestValidator : AbstractValidator<ValidatedRequest>
{
    public ValidatedRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

public static class ValidationFilterFactoryTestFixtures
{
    public static async ValueTask<object?> EndpointWithValidator(ValidatedRequest request)
    {
        await Task.CompletedTask;
        return TypedResults.Ok(new { value = request.Name });
    }

    public static async ValueTask<object?> EndpointWithoutValidator(NoValidatorRequest request)
    {
        await Task.CompletedTask;
        return TypedResults.Ok(new { value = request.Name });
    }
}

public sealed record NoValidatorRequest(string Name);

internal sealed class TestEndpointFilterInvocationContext : EndpointFilterInvocationContext
{
    private readonly object?[] _arguments;

    public TestEndpointFilterInvocationContext(HttpContext httpContext, params object?[] arguments)
    {
        HttpContext = httpContext;
        _arguments = arguments;
    }

    public override HttpContext HttpContext { get; }

    public override IList<object?> Arguments => _arguments;

    public override T GetArgument<T>(int index)
    {
        return (T)_arguments[index]!;
    }
}
