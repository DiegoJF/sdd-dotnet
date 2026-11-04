using System.ComponentModel.DataAnnotations;

namespace RealtorWeb.Features.Properties.Create;

public enum PropertyCreateStateType
{
    Editing,
    Submitting,
    SubmitSucceeded,
    SubmitFailed
}

public enum PropertyStatusOption
{
    Available = 0,
    Rented = 1,
    Maintenance = 2
}

public sealed record CreatePropertyResponse(
    Guid Id,
    string Title,
    string Status,
    string? ImageUrl);

public sealed class CreatePropertyFormModel
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
    public int Status { get; set; } = (int)PropertyStatusOption.Available;

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de dormitorios no puede ser negativa.")]
    public int BedroomCount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "La cantidad de baños no puede ser negativa.")]
    public int BathroomCount { get; set; }

    [Range(typeof(decimal), "1", "79228162514264337593543950335", ErrorMessage = "El area debe ser mayor a 0.")]
    public decimal AreaSquareMeters { get; set; }
}
