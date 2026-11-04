namespace RealtorApi.Features.Properties.DeleteProperty;

public sealed record DeletePropertyCommand(
    string RawId,
    Guid? Id)
{
    public static DeletePropertyCommand Create(string? rawId)
    {
        if (string.IsNullOrWhiteSpace(rawId))
        {
            return new DeletePropertyCommand(string.Empty, null);
        }

        return Guid.TryParseExact(rawId, "D", out var parsedId)
            ? new DeletePropertyCommand(rawId, parsedId)
            : new DeletePropertyCommand(rawId, null);
    }
}
