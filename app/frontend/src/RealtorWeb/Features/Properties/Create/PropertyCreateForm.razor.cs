using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;
using RealtorWeb.Services.Api.Properties;

namespace RealtorWeb.Features.Properties.Create;

public partial class PropertyCreateForm : ComponentBase
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpeg"
    };

    private readonly CreatePropertyFormModel _model = new();
    private readonly List<string> _submitSummaryErrors = [];
    private readonly PropertyStatusOption[] _statusOptions = Enum.GetValues<PropertyStatusOption>();

    private EditContext _editContext = default!;
    private ValidationMessageStore _validationMessageStore = default!;
    private PropertyCreateStateType _state = PropertyCreateStateType.Editing;
    private string? _statusMessage;
    private string? _imagePreviewUrl;
    private string? _selectedImageName;
    private string? _imageValidationError;
    private IBrowserFile? _selectedImage;
    private bool _isSubmitting;

    [Inject] private IPropertiesApi PropertiesApi { get; set; } = default!;

    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
        _validationMessageStore = new ValidationMessageStore(_editContext);
        _editContext.OnFieldChanged += (_, _) =>
        {
            ValidateForm();
            StateHasChanged();
        };
    }

    private string GetStatusLabel(PropertyStatusOption status)
        => status switch
        {
            PropertyStatusOption.Available => "Disponible",
            PropertyStatusOption.Rented => "Rentada",
            PropertyStatusOption.Maintenance => "Mantenimiento",
            _ => status.ToString()
        };

    private async Task OnImageSelectedAsync(InputFileChangeEventArgs args)
    {
        _imageValidationError = null;
        var file = args.File;

        if (!AllowedMimeTypes.Contains(file.ContentType))
        {
            _selectedImage = null;
            _selectedImageName = null;
            _imagePreviewUrl = null;
            _imageValidationError = "Solo se permite imagen PNG o JPEG.";
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "La imagen seleccionada no cumple el formato permitido.";
            return;
        }

        if (file.Size > MaxImageSizeBytes)
        {
            _selectedImage = null;
            _selectedImageName = null;
            _imagePreviewUrl = null;
            _imageValidationError = "La imagen no puede exceder 5 MB.";
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "La imagen seleccionada excede el tamaño permitido.";
            return;
        }

        _selectedImage = file;
        _selectedImageName = file.Name;
        _imagePreviewUrl = await BuildImagePreviewAsync(file);

        if (_state is PropertyCreateStateType.SubmitFailed)
        {
            _state = PropertyCreateStateType.Editing;
            _statusMessage = null;
        }
    }

    private async Task<string?> BuildImagePreviewAsync(IBrowserFile file)
    {
        await using var stream = file.OpenReadStream(MaxImageSizeBytes);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        var base64 = Convert.ToBase64String(memory.ToArray());
        return $"data:{file.ContentType};base64,{base64}";
    }

    private void RemoveImageSelection()
    {
        _selectedImage = null;
        _selectedImageName = null;
        _imagePreviewUrl = null;
        _imageValidationError = null;

        if (_state is PropertyCreateStateType.SubmitFailed)
        {
            _state = PropertyCreateStateType.Editing;
            _statusMessage = null;
        }
    }

    private bool ValidateForm()
    {
        _submitSummaryErrors.Clear();
        _validationMessageStore.Clear();

        var validationContext = new ValidationContext(_model);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(_model, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            if (validationResult.MemberNames.Any())
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    _validationMessageStore.Add(_editContext.Field(memberName), validationResult.ErrorMessage ?? "Valor inválido.");
                }
            }
            else
            {
                _submitSummaryErrors.Add(validationResult.ErrorMessage ?? "Hay errores de validación.");
            }
        }

        if (_imageValidationError is not null)
        {
            isValid = false;
        }

        _editContext.NotifyValidationStateChanged();
        return isValid;
    }

    private async Task HandleValidSubmitAsync()
    {
        _submitSummaryErrors.Clear();

        if (!ValidateForm())
        {
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "Existen validaciones pendientes antes de enviar.";
            _submitSummaryErrors.Add("Revisa los campos marcados antes de continuar.");
            return;
        }

        _isSubmitting = true;
        _state = PropertyCreateStateType.Submitting;
        _statusMessage = "Procesando alta de inmueble...";

        try
        {
            StreamPart? imagePart = null;
            Stream? stream = null;

            if (_selectedImage is not null)
            {
                stream = _selectedImage.OpenReadStream(MaxImageSizeBytes);
                imagePart = new StreamPart(stream, _selectedImage.Name, _selectedImage.ContentType);
            }

            try
            {
                var response = await PropertiesApi.CreatePropertyAsync(
                    _model.Title,
                    _model.Description,
                    _model.Address,
                    _model.Price,
                    _model.Status,
                    _model.BedroomCount,
                    _model.BathroomCount,
                    _model.AreaSquareMeters,
                    imagePart,
                    CancellationToken.None);

                _state = PropertyCreateStateType.SubmitSucceeded;
                _statusMessage = "La propiedad se creó correctamente. Redirigiendo al detalle...";
                StateHasChanged();
                await Task.Delay(650);
                Navigation.NavigateTo(string.Concat("/properties/", response.Id.ToString()));
            }
            finally
            {
                if (stream is not null)
                {
                    await stream.DisposeAsync();
                }
            }
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.BadRequest)
        {
            await ApplyValidationErrorsFromBadRequestAsync(apiException);
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "Corrige los datos del formulario para continuar.";
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.RequestEntityTooLarge)
        {
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "La imagen excede el límite permitido de 5 MB.";
            _submitSummaryErrors.Add("La imagen excede el límite permitido de 5 MB.");
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.UnsupportedMediaType)
        {
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "El tipo de archivo no es compatible. Usa PNG o JPEG.";
            _submitSummaryErrors.Add("El tipo de archivo no es compatible. Usa PNG o JPEG.");
        }
        catch (ApiException)
        {
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "No fue posible completar el alta por un error interno. Intenta nuevamente.";
            _submitSummaryErrors.Add("No fue posible completar el alta por un error interno. Intenta nuevamente.");
        }
        catch (Exception)
        {
            _state = PropertyCreateStateType.SubmitFailed;
            _statusMessage = "No fue posible completar el alta por un error inesperado. Intenta nuevamente.";
            _submitSummaryErrors.Add("No fue posible completar el alta por un error inesperado. Intenta nuevamente.");
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task ApplyValidationErrorsFromBadRequestAsync(ApiException apiException)
    {
        _validationMessageStore.Clear();
        var rawContent = apiException.Content;

        if (string.IsNullOrWhiteSpace(rawContent))
        {
            _submitSummaryErrors.Add("La solicitud no fue aceptada. Revisa los datos e intenta nuevamente.");
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(rawContent);
            if (document.RootElement.TryGetProperty("errors", out var errorsElement)
                && errorsElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in errorsElement.EnumerateObject())
                {
                    var messages = property.Value.EnumerateArray()
                        .Select(static value => value.GetString())
                        .Where(static value => !string.IsNullOrWhiteSpace(value))
                        .Select(static value => value!)
                        .ToArray();

                    if (messages.Length == 0)
                    {
                        continue;
                    }

                    var targetProperty = ResolvePropertyName(property.Name);
                    if (targetProperty is not null)
                    {
                        _validationMessageStore.Add(_editContext.Field(targetProperty), messages);
                    }
                    else
                    {
                        _submitSummaryErrors.AddRange(messages);
                    }
                }
            }
        }
        catch (JsonException)
        {
            _submitSummaryErrors.Add("No fue posible interpretar los errores de validación del backend.");
        }

        if (_submitSummaryErrors.Count == 0)
        {
            _submitSummaryErrors.Add("Corrige los campos marcados antes de reenviar.");
        }

        _editContext.NotifyValidationStateChanged();
    }

    private static string? ResolvePropertyName(string key)
    {
        return key.ToLowerInvariant() switch
        {
            "title" => nameof(CreatePropertyFormModel.Title),
            "description" => nameof(CreatePropertyFormModel.Description),
            "address" => nameof(CreatePropertyFormModel.Address),
            "price" => nameof(CreatePropertyFormModel.Price),
            "status" => nameof(CreatePropertyFormModel.Status),
            "bedroomcount" => nameof(CreatePropertyFormModel.BedroomCount),
            "bathroomcount" => nameof(CreatePropertyFormModel.BathroomCount),
            "areasquaremeters" => nameof(CreatePropertyFormModel.AreaSquareMeters),
            _ => null
        };
    }

    private void ClearGlobalError()
    {
        _submitSummaryErrors.Clear();
        _statusMessage = null;
        _state = PropertyCreateStateType.Editing;
    }
}
