using Microsoft.EntityFrameworkCore;
using RealtorApi.Domain.Properties;
using RealtorApi.Infrastructure.Handlers;
using RealtorApi.Infrastructure.Persistence;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesHandler(AppDbContext dbContext) : IHandler
{
    internal async Task<ListPropertiesPageResult> HandleAsync(
        ListPropertiesQuery query,
        CancellationToken cancellationToken)
    {
        var queryable = dbContext.Properties
            .AsNoTracking()
            .OrderBy(property => property.Title);

        var totalItems = await queryable.CountAsync(cancellationToken);
        var pagination = ListPropertiesPagination.Create(query.Page, query.PageSize, totalItems);

        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(property => new ListPropertiesPropertyRow(
                property.Id,
                property.Title,
                property.Description,
                property.Address,
                property.Price,
                property.Status,
                property.BedroomCount,
                property.BathroomCount,
                property.AreaSquareMeters,
                property.ImageUrl))
            .ToListAsync(cancellationToken);

        return new ListPropertiesPageResult(items, pagination);
    }
}