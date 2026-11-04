namespace RealtorWeb.Features.Properties.Detail;

public sealed record PropertyDetailResponse(
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

public enum PropertyDetailStateType
{
    Loading,
    Success,
    NotFound,
    Error
}
