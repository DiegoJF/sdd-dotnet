using FluentValidation;

namespace RealtorApi.Features.Properties.DeleteProperty;

public sealed class DeletePropertyValidator : AbstractValidator<DeletePropertyCommand>
{
    public DeletePropertyValidator()
    {
        RuleFor(command => command.RawId)
            .NotEmpty()
            .WithMessage("id es obligatorio.")
            .Must(rawId => Guid.TryParseExact(rawId, "D", out _))
            .WithMessage("id debe ser un UUID canónico.");
    }
}
