using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;

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

        var aggregator = new CountryPopulationAggregator(
            new FakeStatService(apiData),
            new FakeDbService(dbData),
            new FakeCountryNormalizationService());

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
    }

    private sealed class FakeStatService : IStatService
    {
        private readonly List<Tuple<string, int>> _data;

        public FakeStatService(List<Tuple<string, int>> data)
        {
            _data = data;
        }

        public List<Tuple<string, int>> GetCountryPopulations() => _data;

        public Task<List<Tuple<string, int>>> GetCountryPopulationsAsync() => Task.FromResult(_data);
    }

    private sealed class FakeDbService : IDbService
    {
        private readonly Dictionary<string, long> _data;

        public FakeDbService(Dictionary<string, long> data)
        {
            _data = data;
        }

        public Task<Dictionary<string, long>> GetCountryPopulationFromDbAsync() => Task.FromResult(_data);
    }

    private sealed class FakeCountryNormalizationService : ICountryNormalizationService
    {
        public string NormalizeCountryName(string countryName)
        {
            return countryName == "United States of America" ? "U.S.A." : countryName;
        }
    }
}
