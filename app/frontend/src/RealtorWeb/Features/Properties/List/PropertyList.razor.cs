using Microsoft.AspNetCore.Components;
using RealtorWeb.Features.Properties.Delete;
using RealtorWeb.Services.Api.Properties;

namespace RealtorWeb.Features.Properties.List;

public enum PropertyListStateType
{
    Loading,
    Empty,
    Error
}

public partial class PropertyList : ComponentBase, IAsyncDisposable
{
    private const int DefaultPageSize = 9;

    private CancellationTokenSource? cancellationTokenSource;
    private PagedPropertyListResponse? response;
    private string? errorMessage;
    private bool isLoading = true;
    private int currentPage = 1;
    private long requestVersion;

    [Inject] private IPropertiesApi PropertiesApi { get; set; } = default!;

    [Inject] private PropertyNoticeService NoticeService { get; set; } = default!;

    private PropertyListItem? pendingDelete;
    private PropertyNotice? currentNotice;

    protected override async Task OnInitializedAsync()
    {
        currentNotice = NoticeService.Consume();
        await LoadPageAsync(currentPage);
    }

    private void RequestDelete(PropertyListItem property)
    {
        pendingDelete = property;
    }

    private void CancelDelete()
    {
        pendingDelete = null;
    }

    private void DismissNotice()
    {
        currentNotice = null;
    }

    private Task HandleDeletedAsync() => CompleteDeleteAsync(PropertyNotice.Deleted());

    private Task HandleNotFoundAsync() => CompleteDeleteAsync(PropertyNotice.NotFound());

    // Recarga la página actual; si queda vacía y no es la primera, muestra la anterior (RF-14).
    private async Task CompleteDeleteAsync(PropertyNotice notice)
    {
        pendingDelete = null;
        currentNotice = notice;

        var targetPage = currentPage;
        if (response is not null && response.Items.Count <= 1 && targetPage > 1)
        {
            targetPage--;
        }

        await LoadPageAsync(targetPage);

        if (response is not null && response.Items.Count == 0 && currentPage > 1)
        {
            await LoadPageAsync(currentPage - 1);
        }
    }

    private bool CanGoNext => response?.HasNext == true;

    private bool CanGoPrevious => response?.HasPrevious == true;

    private async Task LoadPageAsync(int page)
    {
        requestVersion++;
        var version = requestVersion;

        if (cancellationTokenSource is not null)
        {
            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource.Dispose();
        }

        cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        isLoading = true;
        errorMessage = null;
        StateHasChanged();

        try
        {
            var pageResponse = await PropertiesApi.GetPropertiesAsync(page, DefaultPageSize, cancellationToken);

            if (version != requestVersion)
            {
                return;
            }

            response = pageResponse;
            currentPage = pageResponse.Page;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception)
        {
            if (version != requestVersion)
            {
                return;
            }

            response = null;
            errorMessage = "No fue posible recuperar las propiedades. Intenta nuevamente.";
        }
        finally
        {
            if (version == requestVersion)
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }

    private Task ReloadAsync() => LoadPageAsync(currentPage);

    private Task NextAsync()
    {
        if (!CanGoNext)
        {
            return Task.CompletedTask;
        }

        return LoadPageAsync(currentPage + 1);
    }

    private Task PreviousAsync()
    {
        if (!CanGoPrevious)
        {
            return Task.CompletedTask;
        }

        return LoadPageAsync(currentPage - 1);
    }

    public async ValueTask DisposeAsync()
    {
        if (cancellationTokenSource is not null)
        {
            await cancellationTokenSource.CancelAsync();
            cancellationTokenSource.Dispose();
        }
    }
}
