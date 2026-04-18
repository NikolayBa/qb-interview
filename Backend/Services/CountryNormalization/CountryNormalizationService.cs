using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Backend.Services.CountryNormalization
{
	public class CountryNormalizationService : ICountryNormalizationService
	{
		private readonly Dictionary<string, string> _mappings;

		public CountryNormalizationService()
		{
			string configPath = Path.Combine(AppContext.BaseDirectory, "Config", "CountryMappingConfig.json");
			string json = File.ReadAllText(configPath);
			_mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(json,
				new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
				?? [];
		}

		public string NormalizeCountryName(string countryName)
		{
			if (string.IsNullOrWhiteSpace(countryName))
			{
				return string.Empty;
			}

			if (_mappings.TryGetValue(countryName, out var mapped))
			{
				return mapped;
			}

			return countryName;
		}
	}
}
