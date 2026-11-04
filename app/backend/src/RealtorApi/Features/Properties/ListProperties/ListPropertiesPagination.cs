using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 6;
}

public sealed record ListPropertiesPagination(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious)
{
    public static ListPropertiesPagination Create(int page, int pageSize, int totalItems)
    {
        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        return new ListPropertiesPagination(
            page,
            pageSize,
            totalItems,
            totalPages,
            page < totalPages,
            page > 1);
    }
}

internal sealed record ListPropertiesPropertyRow(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatusValue Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string? ImageUrl);

internal sealed record ListPropertiesPageResult(
    IReadOnlyList<ListPropertiesPropertyRow> Items,
    ListPropertiesPagination Pagination);