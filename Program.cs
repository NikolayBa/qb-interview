using Backend;
using Backend.Services.Database;
using Microsoft.Data.Sqlite;
using System;
using System.Data.Common;

Console.WriteLine("Started");
Console.WriteLine("Getting DB Connection...");



db.GetConnection().Close();

//public class CountryPopulation
//{
//	public string CountryName { get; set; }
//	public long TotalPopulation { get; set; }
//}

//var result = conn.Query<CountryPopulation>(@"
//    SELECT co.CountryName, SUM(ci.Population) AS TotalPopulation
//    FROM City ci
//    JOIN State s ON ci.StateId = s.StateId
//    JOIN Country co ON s.CountryId = co.CountryId
//    GROUP BY co.CountryId, co.CountryName;
//");
// add the result to dictionary

// get the instance of the Weather Service

// Go through the result and check if the value exists in the map.
// If it doesnt't, add it 




// the most performant way is to optimize the data on the data layer


// different Parameters


// Index and query setup

// define models for DB
// select the data via join. Dapper?
// add to a dictionary
// foreach in API call to get the data

// compare to the dictionary if it exists

// Output to the console

// unit tests and integration tests

// async? 

// comments in the code

// bonus - export to add to QB


