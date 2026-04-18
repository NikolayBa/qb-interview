using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend
{
	internal class CountryAggretator
	{
		private readonly IStatService statService;
		private readonly IDbManager dbManger;
		public CountryAggretator(IStatService statService,
								ICountryNormalizationService countryNormalizationService,
								IDbManager dbManager)
		{
			this.statService = statService;
		}

		public async Task<void> AggregatePopulationData()
		{
			//Get Data from DB
			DbConnection conn = dbManger.GetConnection();

			if (conn == null)
			{
				throw new Exception("\"Failed to get connection\"");
			}

			//Query.Execute
			//...

			//Dictionary<string, int> countryMapFromDb = await statService.GetCountryPopulationsAsync();

			//Get Data from API
			List<Tuple<string, int>> apiCountryMappings = await statService.GetCountryPopulationsAsync();
			//Normalize Data
			foreach(countryTuple in apiCountryMappings)
			{s
				countryNormalizationService.NormalizeCountryName(countryTuple.)
			}


		}

	}
}
