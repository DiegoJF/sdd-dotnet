using FluentValidation;

namespace RealtorApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdValidator : AbstractValidator<GetPropertyByIdQuery>
{
    public GetPropertyByIdValidator()
    {
        RuleFor(query => query.RawId)
            .NotEmpty()
            .WithMessage("id es obligatorio.")
            .Must(rawId => Guid.TryParseExact(rawId, "D", out _))
            .WithMessage("id debe ser un UUID canónico.");
    }
}
