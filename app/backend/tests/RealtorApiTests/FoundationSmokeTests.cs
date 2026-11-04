namespace RealtorApiTests;

public class FoundationSmokeTests
{
    [Fact]
    public void ProgramType_ExistsForHostBootstrapping()
    {
        Assert.NotNull(typeof(Program));
    }
}
