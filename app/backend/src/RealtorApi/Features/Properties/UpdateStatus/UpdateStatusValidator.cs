using FluentValidation;

namespace RealtorApi.Features.Properties.UpdateStatus;

public sealed class UpdateStatusValidator : AbstractValidator<UpdateStatusRequest>
{
    public UpdateStatusValidator()
    {
        RuleFor(request => request.Status)
            .IsInEnum()
            .WithMessage("status debe ser uno de los valores permitidos.");
    }
}