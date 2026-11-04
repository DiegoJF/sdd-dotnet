using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdateStatus;

internal static class UpdateStatusMapping
{
    public static UpdateStatusCommand ToCommand(this UpdateStatusRequest request, Guid id)
    {
        var status = (PropertyStatusValue)request.Status;

        return new UpdateStatusCommand(id, status);
    }

    public static UpdateStatusResponse ToResponse(this Property property)
    {
        return new UpdateStatusResponse(
            property.Id,
            property.Title,
            property.Status.ToString(),
            property.ImageUrl);
    }
}