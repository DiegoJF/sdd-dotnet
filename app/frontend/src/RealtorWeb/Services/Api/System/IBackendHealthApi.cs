using Refit;

namespace RealtorWeb.Services.Api.System;

public interface IBackendHealthApi
{
    [Get("/health")]
    Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default);
}
