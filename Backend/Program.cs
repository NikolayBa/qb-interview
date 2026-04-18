using Backend;
using Backend.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

Console.WriteLine("Started");
Console.WriteLine("Aggregating country population...");

var stopwatch = Stopwatch.StartNew();

// Build the DI container with all services and the aggregator
var services = new ServiceCollection()
    .AddApplicationServices()
    .BuildServiceProvider();

// Resolve the aggregatot file and run
var aggregator = services.GetRequiredService<CountryPopulationAggregator>();
await aggregator.AggregatePopulationData();

stopwatch.Stop();

Console.WriteLine("Done");
Console.WriteLine($"Execution time: {stopwatch.Elapsed}");


