using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RealtorApi.Infrastructure.Validation;

namespace RealtorApiTests.Infrastructure.Validation;

public class ValidationFilterFactoryTests
{
    [Fact]
    public async Task DetectaValidatorYEjecutaValidacion_CuandoExiste()
    {
        var services = new ServiceCollection();
        services.AddScoped<IValidator<ValidatedRequest>, ValidatedRequestValidator>();

        await using var provider = services.BuildServiceProvider();
        var factory = BuildFilterFactoryContext(provider, nameof(ValidationFilterFactoryTestFixtures.EndpointWithValidator));

        var invokedNext = false;
        EndpointFilterDelegate next = context =>
        {
            invokedNext = true;
            return ValueTask.FromResult<object?>(TypedResults.Ok());
        };

        var filter = ValidationFilterFactory.Create(factory, next);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var invocation = new TestEndpointFilterInvocationContext(httpContext, new ValidatedRequest("ok"));

        var result = await filter(invocation);

        Assert.True(invokedNext);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task HacePassThrough_CuandoNoExisteValidator()
    {
        var services = new ServiceCollection();

        await using var provider = services.BuildServiceProvider();
        var factory = BuildFilterFactoryContext(provider, nameof(ValidationFilterFactoryTestFixtures.EndpointWithoutValidator));

        var invokedNext = false;
        EndpointFilterDelegate next = context =>
        {
            invokedNext = true;
            return ValueTask.FromResult<object?>(TypedResults.Ok());
        };

        var filter = ValidationFilterFactory.Create(factory, next);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var invocation = new TestEndpointFilterInvocationContext(httpContext, new NoValidatorRequest("ok"));

        await filter(invocation);

        Assert.True(invokedNext);
    }

    [Fact]
    public async Task RetornaValidationProblemDetailsCon400_CuandoFallaLaValidacion()
    {
        var services = new ServiceCollection();
        services.AddScoped<IValidator<ValidatedRequest>, ValidatedRequestValidator>();

        await using var provider = services.BuildServiceProvider();
        var factory = BuildFilterFactoryContext(provider, nameof(ValidationFilterFactoryTestFixtures.EndpointWithValidator));

        var invokedNext = false;
        EndpointFilterDelegate next = context =>
        {
            invokedNext = true;
            return ValueTask.FromResult<object?>(TypedResults.Ok());
        };

        var filter = ValidationFilterFactory.Create(factory, next);

        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var invocation = new TestEndpointFilterInvocationContext(httpContext, new ValidatedRequest(string.Empty));

        var result = await filter(invocation);

        Assert.False(invokedNext);

        var responseContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };

        await ((IResult)result!).ExecuteAsync(responseContext);

        Assert.Equal(StatusCodes.Status400BadRequest, responseContext.Response.StatusCode);
        Assert.Equal("application/problem+json", responseContext.Response.ContentType);
    }

    private static EndpointFilterFactoryContext BuildFilterFactoryContext(IServiceProvider provider, string methodName)
    {
        var methodInfo = typeof(ValidationFilterFactoryTestFixtures)
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"No se encontró el método {methodName}.");

        return new EndpointFilterFactoryContext
        {
            ApplicationServices = provider,
            MethodInfo = methodInfo
        };
    }
}
