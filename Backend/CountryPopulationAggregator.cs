using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend
{
	internal class CountryPopulationAggregator
	{
		private readonly IStatService statService;
		private readonly IDbService dbService;
		private readonly ICountryNormalizationService countryNormalizationService;
		private readonly ILogger<CountryPopulationAggregator> logger;

		public CountryPopulationAggregator(IStatService statService,
										IDbService dbService,
										ICountryNormalizationService countryNormalizationService,
										ILogger<CountryPopulationAggregator> logger)
		{
			this.statService = statService;
			this.dbService = dbService;
			this.countryNormalizationService = countryNormalizationService;
			this.logger = logger;
		}

		public async Task AggregatePopulationData()
		{
			logger.LogInformation("------------------Retrieving data:-------------------");

			// Start both tasks simultaneously
			Task<Dictionary<string, long>> countryMapFromDbTask = dbService.GetCountryPopulationFromDbAsync();
			Task<List<Tuple<string, int>>> apiCountryMappingsTask = statService.GetCountryPopulationsAsync();

			// Wait for both tasks to complete in parallel
			await Task.WhenAll(countryMapFromDbTask, apiCountryMappingsTask);

			// Assign results
			Dictionary<string, long> countryMapFromDb = await countryMapFromDbTask;
			logger.LogInformation($"{countryMapFromDb.Count} records retrieved from the database");

			List<Tuple<string, int>> apiCountryMappings = await apiCountryMappingsTask;
			logger.LogInformation($"{apiCountryMappings.Count} records retrieved from the API");

			// Since DB result is the source of truth, we can iterate over the API results in O(N) time
			foreach (Tuple<string, int> countryTuple in apiCountryMappings)
			{
				//Since Data is Incosistent, we need to normalize country names
				string normalizedCountryName = countryNormalizationService.NormalizeCountryName(countryTuple.Item1);

				if (!countryMapFromDb.ContainsKey(normalizedCountryName))
				{
					countryMapFromDb[normalizedCountryName] = countryTuple.Item2;
				}
				else
				{
					logger.LogInformation($"{normalizedCountryName} already exists in db, skipping..");
				}
			}

			foreach (KeyValuePair<string, long> countryEntry in countryMapFromDb.OrderBy(item => item.Key))
			{
				Console.WriteLine($"{countryEntry.Key}: {countryEntry.Value}");
			}
		}

	}
}
