using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdateStatus;

internal sealed record UpdateStatusCommand(
    Guid Id,
    PropertyStatusValue Status);