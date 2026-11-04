using Microsoft.AspNetCore.Http;
using RealtorApi.Features.Properties;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public PropertyStatus Status { get; set; }

    public IFormFile? Image { get; set; }
}

public sealed record UpdatePropertyResponse(
    Guid Id,
    string Title,
    string Status,
    string? ImageUrl);
