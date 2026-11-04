using Microsoft.AspNetCore.Http;
using RealtorApi.Features.Properties;

namespace RealtorApi.Features.Properties.CreateProperty;

public sealed class CreatePropertyRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public PropertyStatus Status { get; set; }

    public int BedroomCount { get; set; }

    public int BathroomCount { get; set; }

    public decimal AreaSquareMeters { get; set; }

    public IFormFile? Image { get; set; }
}

public sealed record CreatePropertyResponse(
    Guid Id,
    string Title,
    string Status,
    string? ImageUrl);
