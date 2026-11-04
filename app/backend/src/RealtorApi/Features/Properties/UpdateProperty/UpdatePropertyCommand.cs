using Microsoft.AspNetCore.Http;
using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdateProperty;

internal sealed record UpdatePropertyCommand(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatusValue Status,
    IFormFile? Image);
