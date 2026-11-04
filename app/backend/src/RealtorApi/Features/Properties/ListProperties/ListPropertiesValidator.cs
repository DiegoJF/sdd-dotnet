using FluentValidation;

namespace RealtorApi.Features.Properties.ListProperties;

public sealed class ListPropertiesValidator : AbstractValidator<ListPropertiesQuery>
{
    public ListPropertiesValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0)
            .WithMessage("page debe ser mayor que 0.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0)
            .WithMessage("pageSize debe ser mayor que 0.");
    }
}