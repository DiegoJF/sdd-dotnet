namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdQuery(
    string RawId,
    Guid? Id)
{
    public static GetPropertyByIdQuery Create(string? rawId)
    {
        if (string.IsNullOrWhiteSpace(rawId))
        {
            return new GetPropertyByIdQuery(string.Empty, null);
        }

        return Guid.TryParseExact(rawId, "D", out var parsedId)
            ? new GetPropertyByIdQuery(rawId, parsedId)
            : new GetPropertyByIdQuery(rawId, null);
    }
}
