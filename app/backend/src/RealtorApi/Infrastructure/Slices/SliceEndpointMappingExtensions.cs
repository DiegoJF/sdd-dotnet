namespace RealtorApi.Infrastructure.Slices;

public static class SliceEndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapSliceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty);
        group.AddEndpointFilterFactory(Validation.ValidationFilterFactory.Create);

        foreach (var slice in app.ServiceProvider.GetServices<ISlice>())
        {
            slice.AddEndpoint(group);
        }

        return app;
    }
}
