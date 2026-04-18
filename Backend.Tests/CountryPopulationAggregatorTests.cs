using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;
using Moq;

namespace Backend.Tests;

public class CountryPopulationAggregatorTests
{
    [Fact]
    public async Task AggregatePopulationData_PrefersDbData_AndAddsMissingApiCountries()
    {
        var dbData = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
        {
            ["U.S.A."] = 330000000,
            ["India"] = 100
        };

        var apiData = new List<Tuple<string, int>>
        {
            Tuple.Create("United States of America", 309349689),
            Tuple.Create("Chile", 17094270)
        };

        var statServiceMock = new Mock<IStatService>(MockBehavior.Strict);
        statServiceMock
            .Setup(service => service.GetCountryPopulationsAsync())
            .ReturnsAsync(apiData);

        var dbServiceMock = new Mock<IDbService>(MockBehavior.Strict);
        dbServiceMock
            .Setup(service => service.GetCountryPopulationFromDbAsync())
            .ReturnsAsync(dbData);

        var countryNormalizationServiceMock = new Mock<ICountryNormalizationService>(MockBehavior.Strict);
        countryNormalizationServiceMock
            .Setup(service => service.NormalizeCountryName("United States of America"))
            .Returns("U.S.A.");
        countryNormalizationServiceMock
            .Setup(service => service.NormalizeCountryName("Chile"))
            .Returns("Chile");

        var aggregator = new CountryPopulationAggregator(
            statServiceMock.Object,
            dbServiceMock.Object,
            countryNormalizationServiceMock.Object);

        using var output = new StringWriter();
        var originalConsoleOut = Console.Out;

        try
        {
            Console.SetOut(output);
            await aggregator.AggregatePopulationData();
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
        }

        var text = output.ToString();

        Assert.Contains("U.S.A.: 330000000", text);
        Assert.DoesNotContain("U.S.A.: 309349689", text);
        Assert.Contains("Chile: 17094270", text);
        Assert.Contains("India: 100", text);

        statServiceMock.Verify(service => service.GetCountryPopulationsAsync(), Times.Once);
        dbServiceMock.Verify(service => service.GetCountryPopulationFromDbAsync(), Times.Once);
        countryNormalizationServiceMock.Verify(service => service.NormalizeCountryName("United States of America"), Times.Once);
        countryNormalizationServiceMock.Verify(service => service.NormalizeCountryName("Chile"), Times.Once);
    }
}
