namespace RealtorApi.Features.Properties.ListProperties;

public sealed record PropertyListItem(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    string Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string? ImageUrl);

public sealed record PagedPropertyListResponse(
    IReadOnlyList<PropertyListItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);