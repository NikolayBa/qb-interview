using Backend.Services.StatService;

namespace Backend.Tests;

public class ConcreteStatServiceTests
{
    [Fact]
    public async Task GetCountryPopulationsAsync_ReturnsSameDataAsSyncMethod()
    {
        var sut = new ConcreteStatService();

        var syncResult = sut.GetCountryPopulations();
        var asyncResult = await sut.GetCountryPopulationsAsync();

        Assert.Equal(syncResult.Count, asyncResult.Count);

        for (var i = 0; i < syncResult.Count; i++)
        {
            Assert.Equal(syncResult[i].Item1, asyncResult[i].Item1);
            Assert.Equal(syncResult[i].Item2, asyncResult[i].Item2);
        }
    }

    [Fact]
    public void GetCountryPopulations_ContainsExpectedUnitedStatesEntry()
    {
        var sut = new ConcreteStatService();

        var result = sut.GetCountryPopulations();

        Assert.Contains(result, tuple =>
            tuple.Item1 == "United States of America" && tuple.Item2 == 309349689);
    }
}
