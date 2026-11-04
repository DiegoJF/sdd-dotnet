using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/{id}", HandleAsync)
            .WithName("GetPropertyById")
            .Produces<PropertyDetailResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        string id,
        GetPropertyByIdHandler handler,
        IValidator<GetPropertyByIdQuery> validator,
        ILogger<GetPropertyByIdSlice> logger,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var query = GetPropertyByIdQuery.Create(id);
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "Consulta de propiedad por id inválido. RawId={RawId}, ErrorCount={ErrorCount}",
                query.RawId,
                validationResult.Errors.Count);

            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.ErrorMessage)
                        .Distinct(StringComparer.Ordinal)
                        .ToArray(),
                    StringComparer.Ordinal);

            var details = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest
            };

            return TypedResults.BadRequest(details);
        }

        var result = await handler.HandleAsync(query, httpContext, cancellationToken);

        if (result.Problem is not null)
        {
            return TypedResults.Problem(
                title: result.Problem.Title,
                detail: result.Problem.Detail,
                statusCode: result.Problem.Status);
        }

        return TypedResults.Ok(result.Response!);
    }
}
