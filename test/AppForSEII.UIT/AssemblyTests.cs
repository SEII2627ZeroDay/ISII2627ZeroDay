namespace AppForSEII.UIT;

public class AssemblyTests
{
    [Fact]
    public void UiTestAssemblyLoads()
    {
        Assert.NotNull(typeof(Shared.UC_UIT));
    }
}