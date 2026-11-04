using RealtorApi.Features.Properties.DeleteProperty;

namespace RealtorApiTests.Features.Properties.DeleteProperty;

public sealed class DeletePropertyValidatorTests
{
    private readonly DeletePropertyValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_Fails_WhenIdIsEmpty(string? rawId)
    {
        var result = _validator.Validate(DeletePropertyCommand.Create(rawId));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "id es obligatorio.");
    }

    [Theory]
    [InlineData("no-es-uuid")]
    [InlineData("a1f0e2d47c814d129e610b7f0b1a1002")]
    [InlineData("{a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1002}")]
    public void Validate_Fails_WhenIdIsNotCanonicalUuid(string rawId)
    {
        var result = _validator.Validate(DeletePropertyCommand.Create(rawId));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage == "id debe ser un UUID canónico.");
    }

    [Fact]
    public void Validate_Succeeds_WhenIdIsCanonicalUuid()
    {
        var command = DeletePropertyCommand.Create("a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1002");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
        Assert.NotNull(command.Id);
    }
}
