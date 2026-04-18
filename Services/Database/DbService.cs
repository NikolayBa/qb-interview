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

    // Most optimal GROUP by to calculate populations from all cities.
    // Since SQLLite returns the Sum result as a double, we need cast it back as INT
    
    private const string CountryPopulationSql = @"
        SELECT
            co.CountryName AS CountryName,
            CAST(SUM(ci.Population) AS INTEGER) AS Population
        FROM City ci
        JOIN State s ON ci.StateId = s.StateId
        JOIN Country co ON s.CountryId = co.CountryId
        GROUP BY co.CountryId, co.CountryName;";

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
