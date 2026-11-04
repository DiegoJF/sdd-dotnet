using Microsoft.AspNetCore.Http;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.CreateProperty;

internal sealed record CreatePropertyCommand(
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatusValue Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    IFormFile? Image);
