using System.IO;

namespace RealtorApi.Features.Properties.ListProperties;

internal static class ListPropertiesMapping
{
    public static PagedPropertyListResponse ToResponse(
        this ListPropertiesPageResult result,
        HttpContext httpContext)
    {
        var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

        var items = result.Items
            .Select(item => new PropertyListItem(
                item.Id,
                item.Title,
                item.Description,
                item.Address,
                item.Price,
                item.Status.ToString(),
                item.BedroomCount,
                item.BathroomCount,
                item.AreaSquareMeters,
                BuildImageUrl(baseUrl, item.ImageUrl)))
            .ToArray();

        return new PagedPropertyListResponse(
            items,
            result.Pagination.Page,
            result.Pagination.PageSize,
            result.Pagination.TotalItems,
            result.Pagination.TotalPages,
            result.Pagination.HasNext,
            result.Pagination.HasPrevious);
    }

    private static string? BuildImageUrl(string baseUrl, string? imageFileName)
    {
        if (string.IsNullOrWhiteSpace(imageFileName))
        {
            return null;
        }

        var fileName = Path.GetFileName(imageFileName);
        return $"{baseUrl}/assets/properties/{fileName}";
    }
}