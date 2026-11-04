namespace RealtorApi.Infrastructure.Slices;

public interface ISlice
{
    void AddEndpoint(IEndpointRouteBuilder app);
}
