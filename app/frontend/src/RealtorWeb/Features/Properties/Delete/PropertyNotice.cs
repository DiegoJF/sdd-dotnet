namespace RealtorWeb.Features.Properties.Delete;

public enum PropertyNoticeKind
{
    Success,
    Info
}

public sealed record PropertyNotice(PropertyNoticeKind Kind, string Message)
{
    public const string DeletedMessage = "La propiedad fue eliminada correctamente.";

    public const string NotFoundMessage = "La propiedad ya no existe.";

    public static PropertyNotice Deleted() => new(PropertyNoticeKind.Success, DeletedMessage);

    public static PropertyNotice NotFound() => new(PropertyNoticeKind.Info, NotFoundMessage);
}
