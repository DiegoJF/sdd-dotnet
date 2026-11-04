using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;
using RealtorWeb.Services.Api.Properties;

namespace RealtorWeb.Features.Properties.Update;

public partial class PropertyUpdateForm : ComponentBase, IAsyncDisposable
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpeg"
    };

    private readonly PropertyUpdateFormModel _model = new();
    private readonly List<string> _submitSummaryErrors = [];
    private readonly PropertyUpdateStatusOption[] _statusOptions = Enum.GetValues<PropertyUpdateStatusOption>();

    private EditContext _editContext = default!;
    private ValidationMessageStore _validationMessageStore = default!;
    private PropertyUpdateStateType _state = PropertyUpdateStateType.LoadingInitialData;
    private string? _statusMessage;
    private string? _currentImageUrl;
    private string? _imagePreviewUrl;
    private string? _selectedImageName;
    private string? _imageValidationError;
    private IBrowserFile? _selectedImage;
    private bool _isSubmitting;
    private bool _isInitialized;
    private long _requestVersion;
    private CancellationTokenSource? _cancellationTokenSource;

    [Parameter]
    public string Id { get; set; } = default!;

    [Inject] private IPropertiesApi PropertiesApi { get; set; } = default!;

    [Inject] private NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (!_isInitialized)
        {
            InitializeEditContext();
            _isInitialized = true;
        }

        await LoadPropertyAsync();
    }

    private void InitializeEditContext()
    {
        _editContext = new EditContext(_model);
        _validationMessageStore = new ValidationMessageStore(_editContext);
        _editContext.OnFieldChanged += OnFieldChanged;
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs args)
    {
        ValidateForm();
        StateHasChanged();
    }

    private string GetStatusLabel(PropertyUpdateStatusOption status)
        => status switch
        {
            PropertyUpdateStatusOption.Available => "Disponible",
            PropertyUpdateStatusOption.Rented => "Rentada",
            PropertyUpdateStatusOption.Maintenance => "Mantenimiento",
            _ => status.ToString()
        };

    private async Task LoadPropertyAsync()
    {
        _requestVersion++;
        var version = _requestVersion;

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        _state = PropertyUpdateStateType.LoadingInitialData;
        _statusMessage = "Recuperando datos actuales para edición...";
        _submitSummaryErrors.Clear();
        StateHasChanged();

        try
        {
            var detail = await PropertiesApi.GetPropertyByIdAsync(Id, cancellationToken);

            if (version != _requestVersion)
            {
                return;
            }

            _model.Title = detail.Title;
            _model.Description = detail.Description;
            _model.Address = detail.Address;
            _model.Price = detail.Price;
            _model.Status = PropertyUpdateMappings.MapStatusToOption(detail.Status);

            _currentImageUrl = detail.ImageUrl;
            _selectedImage = null;
            _selectedImageName = null;
            _imagePreviewUrl = null;
            _imageValidationError = null;

            _validationMessageStore.Clear();
            _editContext.NotifyValidationStateChanged();

            _state = PropertyUpdateStateType.Editing;
            _statusMessage = null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
        {
            if (version != _requestVersion)
            {
                return;
            }

            _state = PropertyUpdateStateType.LoadFailed;
            _statusMessage = "No se encontró el inmueble solicitado para editar.";
        }
        catch (Exception)
        {
            if (version != _requestVersion)
            {
                return;
            }

            _state = PropertyUpdateStateType.LoadFailed;
            _statusMessage = "No fue posible cargar los datos del inmueble. Intenta nuevamente.";
        }
        finally
        {
            if (version == _requestVersion)
            {
                StateHasChanged();
            }
        }
    }

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
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "La imagen seleccionada no cumple el formato permitido.";
            return;
        }

        if (file.Size > MaxImageSizeBytes)
        {
            _selectedImage = null;
            _selectedImageName = null;
            _imagePreviewUrl = null;
            _imageValidationError = "La imagen no puede exceder 5 MB.";
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "La imagen seleccionada excede el tamaño permitido.";
            return;
        }

        _selectedImage = file;
        _selectedImageName = file.Name;
        _imagePreviewUrl = await BuildImagePreviewAsync(file);

        if (_state is PropertyUpdateStateType.SubmitFailed)
        {
            _state = PropertyUpdateStateType.Editing;
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

        if (_state is PropertyUpdateStateType.SubmitFailed)
        {
            _state = PropertyUpdateStateType.Editing;
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
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "Existen validaciones pendientes antes de enviar.";
            _submitSummaryErrors.Add("Revisa los campos marcados antes de continuar.");
            return;
        }

        _isSubmitting = true;
        _state = PropertyUpdateStateType.Submitting;
        _statusMessage = "Guardando cambios del inmueble...";

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
                var response = await PropertiesApi.UpdatePropertyAsync(
                    Id,
                    _model.Title,
                    _model.Description,
                    _model.Address,
                    _model.Price,
                    _model.Status,
                    imagePart,
                    CancellationToken.None);

                _state = PropertyUpdateStateType.SubmitSucceeded;
                _statusMessage = "La propiedad se actualizó correctamente. Redirigiendo al detalle...";
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
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "Corrige los datos del formulario para continuar.";
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
        {
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "El inmueble ya no está disponible para actualizar.";
            _submitSummaryErrors.Add("El inmueble ya no está disponible para actualizar.");
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.RequestEntityTooLarge)
        {
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "La imagen excede el límite permitido de 5 MB.";
            _submitSummaryErrors.Add("La imagen excede el límite permitido de 5 MB.");
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.UnsupportedMediaType)
        {
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "El tipo de archivo no es compatible. Usa PNG o JPEG.";
            _submitSummaryErrors.Add("El tipo de archivo no es compatible. Usa PNG o JPEG.");
        }
        catch (ApiException)
        {
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "No fue posible actualizar la propiedad por un error interno. Intenta nuevamente.";
            _submitSummaryErrors.Add("No fue posible actualizar la propiedad por un error interno. Intenta nuevamente.");
        }
        catch (Exception)
        {
            _state = PropertyUpdateStateType.SubmitFailed;
            _statusMessage = "No fue posible actualizar la propiedad por un error inesperado. Intenta nuevamente.";
            _submitSummaryErrors.Add("No fue posible actualizar la propiedad por un error inesperado. Intenta nuevamente.");
        }
        finally
        {
            _isSubmitting = false;
            StateHasChanged();
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

                    var targetProperty = PropertyUpdateMappings.ResolvePropertyName(property.Name);
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
        await Task.CompletedTask;
    }

    private void ClearGlobalError()
    {
        _submitSummaryErrors.Clear();
        _statusMessage = null;
        _state = PropertyUpdateStateType.Editing;
    }

    private Task RetryLoadAsync() => LoadPropertyAsync();

    private void BackToList() => Navigation.NavigateTo("/");

    public async ValueTask DisposeAsync()
    {
        if (_editContext is not null)
        {
            _editContext.OnFieldChanged -= OnFieldChanged;
        }

        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
        }
    }
}
