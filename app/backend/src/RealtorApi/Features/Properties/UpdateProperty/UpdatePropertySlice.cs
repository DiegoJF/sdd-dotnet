using Microsoft.AspNetCore.Mvc;
using RealtorApi.Infrastructure.Slices;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertySlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/properties/{id:guid}", HandleAsync)
            .WithName("UpdateProperty")
            .Produces<UpdatePropertyResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .DisableAntiforgery();
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromForm] UpdatePropertyRequest request,
        UpdatePropertyHandler handler,
        CancellationToken cancellationToken)
    {
        var imageValidationError = UpdatePropertyImageValidator.Validate(request.Image);

        if (imageValidationError == UpdatePropertyImageValidationError.UnsupportedMediaType)
        {
            return TypedResults.Problem(
                title: "Formato de imagen no soportado.",
                detail: "La imagen debe ser PNG/JPG válido y no puede estar vacía.",
                statusCode: StatusCodes.Status415UnsupportedMediaType);
        }

        if (imageValidationError == UpdatePropertyImageValidationError.PayloadTooLarge)
        {
            return TypedResults.Problem(
                title: "Imagen demasiado grande.",
                detail: "La imagen no puede superar 5 MB.",
                statusCode: StatusCodes.Status413PayloadTooLarge);
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

        if (result.StorageFailed)
        {
            return TypedResults.Problem(
                title: "Error interno al guardar la imagen.",
                detail: "No fue posible completar la actualización de propiedad.",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return TypedResults.Ok(result.Response!);
    }
}
