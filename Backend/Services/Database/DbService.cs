using Backend.Models;
using Backend.Services.CountryNormalization;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services.Database;

public class DbService : IDbService
{
    // Most optimal db query - GROUP BY to calculate populations from all countries, by joining cities and states
    // Since SQLLite returns the Sum result as a double, we need cast it back as INT
    // using interpolation with model property names for columns for easier maintenance
    
    private static readonly string CountryPopulationSql = $@"
        SELECT
            co.{nameof(Country.CountryName)} AS {nameof(CountryPopulationAggregate.CountryName)},
            CAST(SUM(ci.{nameof(City.Population)}) AS INTEGER) AS {nameof(CountryPopulationAggregate.Population)}
        FROM {nameof(City)} ci
        JOIN {nameof(State)} s ON ci.{nameof(City.StateId)} = s.{nameof(State.StateId)}
        JOIN {nameof(Country)} co ON s.{nameof(State.CountryId)} = co.{nameof(Country.CountryId)}
        GROUP BY co.{nameof(Country.CountryId)}, co.{nameof(Country.CountryName)};";

    private readonly IDbManager dbManager;
    private readonly ICountryNormalizationService countryNormalizationService;

    public DbService(IDbManager dbManager, ICountryNormalizationService countryNormalizationService)
    {
        this.dbManager = dbManager;
        this.countryNormalizationService = countryNormalizationService;
    }

    public async Task<Dictionary<string, long>> GetCountryPopulationFromDbAsync()
    {
        using DbConnection connection = dbManager.GetConnection()
            ?? throw new InvalidOperationException("Failed to get DB connection.");

        IEnumerable<CountryPopulationAggregate> countryPopulationRows =
            await connection.QueryAsync<CountryPopulationAggregate>(CountryPopulationSql);

        return countryPopulationRows.ToDictionary(
            item => countryNormalizationService.NormalizeCountryName(item.CountryName),
            item => item.Population,
            StringComparer.OrdinalIgnoreCase);
    }
}
