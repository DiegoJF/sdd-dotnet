using RealtorApi.Domain.Properties;

namespace RealtorApi.Features.Properties.CreateProperty;

internal static class CreatePropertyMapping
{
    public static CreatePropertyCommand ToCommand(this CreatePropertyRequest request)
    {
        var status = (PropertyStatusValue)request.Status;

        return new CreatePropertyCommand(
            request.Title.Trim(),
            request.Description.Trim(),
            request.Address.Trim(),
            request.Price,
            status,
            request.BedroomCount,
            request.BathroomCount,
            request.AreaSquareMeters,
            request.Image);
    }

    public static Property ToProperty(
        this CreatePropertyCommand command,
        string? imageUrl,
        DateTimeOffset utcNow)
    {
        return new Property
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Address = command.Address,
            Price = command.Price,
            Status = command.Status,
            BedroomCount = command.BedroomCount,
            BathroomCount = command.BathroomCount,
            AreaSquareMeters = command.AreaSquareMeters,
            ImageUrl = imageUrl,
            CreatedAt = utcNow
        };
    }

    public static CreatePropertyResponse ToResponse(this Property property)
    {
        return new CreatePropertyResponse(
            property.Id,
            property.Title,
            property.Status.ToString(),
            property.ImageUrl);
    }
}
