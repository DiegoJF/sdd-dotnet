using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.GetPropertyById;

internal static class GetPropertyByIdMapping
{
    public static PropertyDetailResponse ToResponse(this Property property, HttpContext httpContext)
    {
        var baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

        return new PropertyDetailResponse(
            property.Id,
            property.Title,
            property.Description,
            property.Address,
            property.Price,
            property.Status.ToString(),
            property.BedroomCount,
            property.BathroomCount,
            property.AreaSquareMeters,
            BuildImageUrl(baseUrl, property.ImageUrl));
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
