using System.ComponentModel.DataAnnotations;

namespace RealtorWeb.Features.Properties.Update;

public enum PropertyUpdateStateType
{
    LoadingInitialData,
    LoadFailed,
    Editing,
    Submitting,
    SubmitSucceeded,
    SubmitFailed
}

public enum PropertyUpdateStatusOption
{
    Available = 0,
    Rented = 1,
    Maintenance = 2
}

public sealed record UpdatePropertyResponse(
    Guid Id,
    string Title,
    string Status,
    string? ImageUrl);

public sealed class PropertyUpdateFormModel
{
    [Required(ErrorMessage = "El titulo es obligatorio.")]
    [MinLength(3, ErrorMessage = "El titulo debe tener al menos 3 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripcion es obligatoria.")]
    [MinLength(10, ErrorMessage = "La descripcion debe tener al menos 10 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "La direccion es obligatoria.")]
    [MinLength(5, ErrorMessage = "La direccion debe tener al menos 5 caracteres.")]
    public string Address { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "79228162514264337593543950335", ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal Price { get; set; }

    [Range(0, 2, ErrorMessage = "El estado seleccionado no es válido.")]
    public int Status { get; set; } = (int)PropertyUpdateStatusOption.Available;
}

public static class PropertyUpdateMappings
{
    public static int MapStatusToOption(string status)
    {
        if (Enum.TryParse<PropertyUpdateStatusOption>(status, true, out var parsed))
        {
            return (int)parsed;
        }

        return (int)PropertyUpdateStatusOption.Available;
    }

    public static string? ResolvePropertyName(string key)
    {
        return key.ToLowerInvariant() switch
        {
            "title" => nameof(PropertyUpdateFormModel.Title),
            "description" => nameof(PropertyUpdateFormModel.Description),
            "address" => nameof(PropertyUpdateFormModel.Address),
            "price" => nameof(PropertyUpdateFormModel.Price),
            "status" => nameof(PropertyUpdateFormModel.Status),
            _ => null
        };
    }
}
