using System.Net;
using Microsoft.AspNetCore.Components;
using Refit;
using RealtorWeb.Services.Api.Properties;

namespace RealtorWeb.Features.Properties.Delete;

public partial class PropertyDeleteDialog : ComponentBase
{
    public const string GenericErrorMessage = "No se pudo eliminar la propiedad. Inténtalo de nuevo.";

    private bool isDeleting;
    private string? errorMessage;
    private bool wasOpen;

    [Parameter, EditorRequired]
    public string PropertyId { get; set; } = string.Empty;

    [Parameter, EditorRequired]
    public string PropertyTitle { get; set; } = string.Empty;

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback OnCancel { get; set; }

    [Parameter]
    public EventCallback OnDeleted { get; set; }

    [Parameter]
    public EventCallback OnNotFound { get; set; }

    [Inject] private IPropertiesApi PropertiesApi { get; set; } = default!;

    protected override void OnParametersSet()
    {
        // Cada apertura del diálogo comienza sin errores previos.
        if (IsOpen && !wasOpen)
        {
            errorMessage = null;
            isDeleting = false;
        }

        wasOpen = IsOpen;
    }

    private async Task CancelAsync()
    {
        if (isDeleting)
        {
            return;
        }

        errorMessage = null;
        await OnCancel.InvokeAsync();
    }

    private async Task ConfirmAsync()
    {
        // Evita solicitudes duplicadas mientras una eliminación está en curso (RF-13).
        if (isDeleting)
        {
            return;
        }

        isDeleting = true;
        errorMessage = null;

        try
        {
            await PropertiesApi.DeletePropertyAsync(PropertyId);
            isDeleting = false;
            await OnDeleted.InvokeAsync();
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
        {
            isDeleting = false;
            await OnNotFound.InvokeAsync();
        }
        catch (Exception exception) when (exception is ApiException or HttpRequestException or TaskCanceledException)
        {
            // El diálogo permanece abierto y permite reintentar o cancelar (RF-15).
            isDeleting = false;
            errorMessage = GenericErrorMessage;
        }
    }
}
