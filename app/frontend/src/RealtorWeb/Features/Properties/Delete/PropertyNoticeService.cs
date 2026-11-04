namespace RealtorWeb.Features.Properties.Delete;

/// <summary>
/// Conserva un aviso de un solo uso entre páginas del mismo circuito.
/// </summary>
public sealed class PropertyNoticeService
{
    private PropertyNotice? pending;

    public void Set(PropertyNotice notice) => pending = notice;

    public PropertyNotice? Consume()
    {
        var notice = pending;
        pending = null;
        return notice;
    }
}
