using Backend;
using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;
using System;

Console.WriteLine("Started");
Console.WriteLine("Aggregating country population...");

IStatService statService = new ConcreteStatService();
IDbManager dbManager = new SqliteDbManager();
ICountryNormalizationService countryNormalizationService = new CountryNormalizationService();

CountryPopulationAggretator aggregator = new CountryPopulationAggretator(
	statService,
	dbManager,
	countryNormalizationService);

await aggregator.AggregatePopulationData();

Console.WriteLine("Done");


