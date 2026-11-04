using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.UpdateProperty;

internal static class UpdatePropertyMapping
{
    public static UpdatePropertyCommand ToCommand(this UpdatePropertyRequest request, Guid id)
    {
        var status = (PropertyStatusValue)request.Status;

        return new UpdatePropertyCommand(
            id,
            request.Title.Trim(),
            request.Description.Trim(),
            request.Address.Trim(),
            request.Price,
            status,
            request.Image);
    }

    public static UpdatePropertyResponse ToResponse(this Property property)
    {
        return new UpdatePropertyResponse(
            property.Id,
            property.Title,
            property.Status.ToString(),
            property.ImageUrl);
    }
}
