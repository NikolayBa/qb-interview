using Backend.Models;
using Backend.Services.CountryNormalization;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
	private readonly ILogger<DbService> logger;

	public DbService(
		IDbManager dbManager,
		ICountryNormalizationService countryNormalizationService,
		ILogger<DbService> logger)
	{
		this.dbManager = dbManager;
		this.countryNormalizationService = countryNormalizationService;
		this.logger = logger;
	}

	public async Task<Dictionary<string, long>> GetCountryPopulationFromDbAsync()
	{
		using DbConnection connection = dbManager.GetConnection()
			?? throw new InvalidOperationException("Failed to establish DB connection. App needs DB to run");

		IEnumerable<CountryPopulationAggregate> countryPopulationRows =
			await connection.QueryAsync<CountryPopulationAggregate>(CountryPopulationSql);

		var result = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

		foreach (CountryPopulationAggregate item in countryPopulationRows)
		{
			string normalizedName = countryNormalizationService.NormalizeCountryName(item.CountryName);

			if (string.IsNullOrWhiteSpace(normalizedName))
			{
				logger.LogWarning(
					"Skipping empty country name value. Raw country name: {CountryName}",
					item.CountryName);
				continue;
			}

			if (!result.TryAdd(normalizedName, item.Population))
			{
				logger.LogWarning(
					"Duplicate normalized country key in db detected, skipping. Raw country name: {CountryName}, Normalized: {NormalizedName}",
					item.CountryName,
					normalizedName);
			}
		}

		return result;
	}
}
