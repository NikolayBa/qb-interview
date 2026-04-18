using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;
using Backend.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
	internal class CountryPopulationAggretator
	{
		private const string CountryPopulationSql = @"
			SELECT
				co.CountryName AS CountryName,
				CAST(SUM(ci.Population) AS INTEGER) AS Population
			FROM City ci
			JOIN State s ON ci.StateId = s.StateId
			JOIN Country co ON s.CountryId = co.CountryId
			GROUP BY co.CountryId, co.CountryName;";

		private readonly IStatService statService;
		private readonly IDbManager dbManger;
		private readonly ICountryNormalizationService countryNormalizationService;
		public CountryPopulationAggretator(IStatService statService,
										IDbManager dbManager,
										ICountryNormalizationService countryNormalizationService)
		{
			this.statService = statService;
			this.dbManger = dbManager;
			this.countryNormalizationService = countryNormalizationService;
		}

		public async Task AggregatePopulationData()
		{
			Dictionary<string, long> countryMapFromDb = await GetCountryPopulationFromDbAsync();
			List<Tuple<string, int>> apiCountryMappings = await statService.GetCountryPopulationsAsync();

			foreach (Tuple<string, int> countryTuple in apiCountryMappings)
			{
				string normalizedCountryName = countryNormalizationService.NormalizeCountryName(countryTuple.Item1);

				if (!countryMapFromDb.ContainsKey(normalizedCountryName))
				{
					countryMapFromDb[normalizedCountryName] = countryTuple.Item2;
				}
			}

			foreach (KeyValuePair<string, long> countryEntry in countryMapFromDb.OrderBy(item => item.Key))
			{
				Console.WriteLine($"{countryEntry.Key}: {countryEntry.Value}");
			}
		}

		private async Task<Dictionary<string, long>> GetCountryPopulationFromDbAsync()
		{
			using DbConnection connection = dbManger.GetConnection()
				?? throw new InvalidOperationException("Failed to get DB connection.");

			IEnumerable<CountryPopulationAggregate> countryPopulationRows =
				await connection.QueryAsync<CountryPopulationAggregate>(CountryPopulationSql);

			return countryPopulationRows.ToDictionary(
				item => countryNormalizationService.NormalizeCountryName(item.CountryName),
				item => item.Population,
				StringComparer.OrdinalIgnoreCase);
		}

	}
}
