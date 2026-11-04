using Microsoft.AspNetCore.Mvc;
using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Features.Properties.CreateProperty;

public sealed class CreatePropertySlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/properties", HandleAsync)
            .WithName("CreateProperty")
            .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .DisableAntiforgery();
    }

    private static async Task<IResult> HandleAsync(
        [FromForm] CreatePropertyRequest request,
        CreatePropertyHandler handler,
        CancellationToken cancellationToken)
    {
        var imageValidationError = CreatePropertyImageValidator.Validate(request.Image);

        if (imageValidationError == CreatePropertyImageValidationError.UnsupportedMediaType)
        {
            return TypedResults.Problem(
                title: "Formato de imagen no soportado.",
                detail: "La imagen debe ser PNG/JPG válido y no puede estar vacía.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        if (imageValidationError == CreatePropertyImageValidationError.PayloadTooLarge)
        {
            return TypedResults.Problem(
                title: "Imagen demasiado grande.",
                detail: "La imagen no puede superar 5 MB.",
                statusCode: StatusCodes.Status413PayloadTooLarge);
        }

        var command = request.ToCommand();
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.StorageFailed)
        {
            return TypedResults.Problem(
                title: "Error interno al guardar la imagen.",
                detail: "No fue posible completar el alta de propiedad.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var response = result.Response!;
        return TypedResults.Created($"/api/properties/{response.Id}", response);
    }
}
