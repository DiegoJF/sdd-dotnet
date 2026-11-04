using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace RealtorApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(160)
            .WithMessage("title es obligatorio y debe tener como máximo 160 caracteres.");

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(1200)
            .WithMessage("description es obligatorio y debe tener como máximo 1200 caracteres.");

        RuleFor(request => request.Address)
            .NotEmpty()
            .MaximumLength(240)
            .WithMessage("address es obligatorio y debe tener como máximo 240 caracteres.");

        RuleFor(request => request.Price)
            .GreaterThan(0)
            .WithMessage("price debe ser mayor que 0.");

        RuleFor(request => request.Status)
            .IsInEnum()
            .WithMessage("status debe ser uno de los valores permitidos.");
    }
}

internal enum UpdatePropertyImageValidationError
{
    None,
    UnsupportedMediaType,
    PayloadTooLarge
}

internal static class UpdatePropertyImageValidator
{
    internal const long MaxImageBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".png",
        ".jpg",
        ".jpeg"
    ];

    public static UpdatePropertyImageValidationError Validate(IFormFile? image)
    {
        if (image is null)
        {
            return UpdatePropertyImageValidationError.None;
        }

        if (image.Length <= 0)
        {
            return UpdatePropertyImageValidationError.UnsupportedMediaType;
        }

        if (image.Length > MaxImageBytes)
        {
            return UpdatePropertyImageValidationError.PayloadTooLarge;
        }

        var extension = Path.GetExtension(image.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension.ToLowerInvariant()))
        {
            return UpdatePropertyImageValidationError.UnsupportedMediaType;
        }

        if (!HasSupportedImageSignature(image))
        {
            return UpdatePropertyImageValidationError.UnsupportedMediaType;
        }

        return UpdatePropertyImageValidationError.None;
    }

    private static bool HasSupportedImageSignature(IFormFile image)
    {
        Span<byte> signature = stackalloc byte[8];

        using var stream = image.OpenReadStream();
        var bytesRead = stream.Read(signature);

        if (bytesRead < 3)
        {
            return false;
        }

        var isJpeg = signature[0] == 0xFF &&
                     signature[1] == 0xD8 &&
                     signature[2] == 0xFF;

        if (isJpeg)
        {
            return true;
        }

        if (bytesRead < 8)
        {
            return false;
        }

        var isPng = signature[0] == 0x89 &&
                    signature[1] == 0x50 &&
                    signature[2] == 0x4E &&
                    signature[3] == 0x47 &&
                    signature[4] == 0x0D &&
                    signature[5] == 0x0A &&
                    signature[6] == 0x1A &&
                    signature[7] == 0x0A;

        return isPng;
    }
}
