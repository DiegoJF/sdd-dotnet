using Refit;
using RealtorWeb.Features.Properties.Create;
using RealtorWeb.Features.Properties.Detail;
using RealtorWeb.Features.Properties.List;
using RealtorWeb.Features.Properties.Update;

namespace RealtorWeb.Services.Api.Properties;

public interface IPropertiesApi
{
    [Get("/api/properties")]
    Task<PagedPropertyListResponse> GetPropertiesAsync(
        [Query("Page")] int page,
        [Query("PageSize")] int pageSize,
        CancellationToken cancellationToken = default);

    [Get("/api/properties/{id}")]
    Task<PropertyDetailResponse> GetPropertyByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    [Multipart]
    [Post("/api/properties")]
    Task<CreatePropertyResponse> CreatePropertyAsync(
        [AliasAs("title")] string title,
        [AliasAs("description")] string description,
        [AliasAs("address")] string address,
        [AliasAs("price")] decimal price,
        [AliasAs("status")] int status,
        [AliasAs("bedroomCount")] int bedroomCount,
        [AliasAs("bathroomCount")] int bathroomCount,
        [AliasAs("areaSquareMeters")] decimal areaSquareMeters,
        [AliasAs("image")] StreamPart? image = null,
        CancellationToken cancellationToken = default);

    [Multipart]
    [Put("/api/properties/{id}")]
    Task<UpdatePropertyResponse> UpdatePropertyAsync(
        string id,
        [AliasAs("title")] string title,
        [AliasAs("description")] string description,
        [AliasAs("address")] string address,
        [AliasAs("price")] decimal price,
        [AliasAs("status")] int status,
        [AliasAs("image")] StreamPart? image = null,
        CancellationToken cancellationToken = default);
}
