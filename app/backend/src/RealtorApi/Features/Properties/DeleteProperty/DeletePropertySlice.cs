using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Features.Properties.DeleteProperty;

public sealed class DeletePropertySlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/properties/{id}", HandleAsync)
            .WithName("DeleteProperty")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        string id,
        DeletePropertyHandler handler,
        IValidator<DeletePropertyCommand> validator,
        ILogger<DeletePropertySlice> logger,
        CancellationToken cancellationToken)
    {
        var command = DeletePropertyCommand.Create(id);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "Eliminación de propiedad con id inválido. RawId={RawId}, ErrorCount={ErrorCount}",
                command.RawId,
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

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.Problem is not null)
        {
            return TypedResults.Problem(
                title: result.Problem.Title,
                detail: result.Problem.Detail,
                statusCode: result.Problem.Status);
        }

        return TypedResults.NoContent();
    }
}
