using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Features.Properties.UpdateStatus;

public sealed class UpdateStatusSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/properties/{id:guid}/status", HandleAsync)
            .WithName("UpdatePropertyStatus")
            .Produces<UpdateStatusResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdateStatusRequest request,
        UpdateStatusHandler handler,
        IValidator<UpdateStatusRequest> validator,
        ILogger<UpdateStatusSlice> logger,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning(
                "Solicitud de actualización de estado inválida para propiedad '{PropertyId}'. ErrorCount={ErrorCount}",
                id,
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

        var command = request.ToCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.NotFound)
        {
            return TypedResults.Problem(
                title: "Propiedad no encontrada.",
                detail: "No existe una propiedad con el id proporcionado.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return TypedResults.Ok(result.Response!);
    }
}