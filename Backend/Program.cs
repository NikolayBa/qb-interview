using Backend;
using Backend.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Diagnostics;

Console.WriteLine("Started");
Console.WriteLine("Aggregating country population...");

var stopwatch = Stopwatch.StartNew();

// Build the DI container with all services and the aggregator
using var services = new ServiceCollection()
	.AddApplicationLogging()
	.AddApplicationServices()
	.BuildServiceProvider();

// Resolve the aggregatot file and run
var aggregator = services.GetRequiredService<CountryPopulationAggregator>();
await aggregator.AggregatePopulationData();

stopwatch.Stop();

Console.WriteLine("Completed");
Console.WriteLine($"Execution time: {stopwatch.Elapsed}");

// Using Static Logger class outside of DI testable class
Log.Information($"----------Execution time: {stopwatch.Elapsed}-----------");

Log.CloseAndFlush();


