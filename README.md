# Solution for #githublink Quickbase interview
# Nikolay Babev
agenda

Problem
Assumptions, consideration


Challanges:
Data aggegation differences
SQLLite
Lightweight solution
Code clarity

Solution:

o(n + m)
Parsing Aggregator

Updating

Best Practices that were followed:
Using Dapper Instead of EF, as it's more lightweight

Using Async/Await and Tasks on services - eg the DB call and the API call are happening simultaneously

DI for easier testing, and modern best practice

Using Dictionary for O(1) for fast o(1) access

Mapping values based on a Config
Extendable 

Bonus:
IncludeInTerritories 

parameters
includeTerritories

unit tests
Integration Tests

comments in the code

How to run the project:

Prepare (In a real project this step would be automated with a migration strategy)
run the project as --prepareDb parameter
or Open apply the query directly to the db


Run:

Run the project
Get an output
Save in the database

Unit/Integration Tests

Bonus feature:
export to qb

#Solution:

The solution 
Indexes are added to optimize the query execution. On the current dataset they are optional, but suitable for big projects
New Table is added to the database to save the results for further execution

For DB access, Dapper library is used as more lightwight version



# Materials for developer interviews for Quickbase

#Assumptions:

Data consitency - Country Data is usually stored in

The key to the solution is that the data from the DB is the source of trut

Overseas territories are counted as separate entities. Eg Mayotte's population is not counted towards France mainland population, as national statisctics.



## Requirements
The project requirement is to aggregate data (in this case population statistics) from two disparate sources.
We've provided two classes to represent those sources. `SqliteDbManager.cs`, provides access to a SQL database containing population
data for cities.  Each city is in a state within a country.  You need to write a method to retrieve the total
population for each country.  The other class, `IStatService.cs`, returns a `List<Tuple<String, Integer>>` containing 
country population data. For the purposes of this exercise, we've provided a concrete class that just returns a 
hard-coded list, but in a real project, assume it would be calling an API.

The assignment is to implement a solution that consumes these two data sources and returns the combined list of
countries and their populations. In the event of duplicate population data for a given country, the data from
the sql database should be used. 

## Building and Running the code

This project assumes you're using Visual Studio 2022 or newer and depends on nuget.

That said, feel free to challenge any of the current limitations with your demo. Just keep the time limit in mind.
