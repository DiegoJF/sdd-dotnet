using System.Text.Json.Serialization;
using RealtorApi.Features.Properties;

namespace RealtorApi.Features.Properties.UpdateStatus;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class UpdateStatusRequest
{
    public required PropertyStatus Status { get; set; }
}

public sealed record UpdateStatusResponse(
    Guid Id,
    string Title,
    string Status,
    string? ImageUrl);