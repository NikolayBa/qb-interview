## Solution for Quickbase interview task  https://github.com/QuickBase/interview-demos/tree/master/c%23

## Author: Nikolay Babev

##  Functional requirements:
Merge population data from different data sources, while keeping the source of truth as the db

## Non-functional requirements:
 - Fast, performant and lightweight solution
 - Handle data differences 
 - Clear and understandable code using best practices 


## Solution Implementation:
The implementation is achieved using a dictionary to map the values based with time complexity
o(n + m) where n is the database size and m is the api results size.

Sorting the country alphabetically for clear output (sorting all countries and territories would be fast) 

### Best Practices that were followed:
- Using Dapper and Raw SQL Instead of EF for a quick and lightweight query and db mapping

- Using Async/Await and Tasks on services - eg the DB call and the API call are happening simultaneously

- DI for easier testing, and modern best practices

- Data normalization and mapping controlled via a separate config

- Unit test using xUnit

- Serilog for logging

- Clear folder structure and comments in the code
 
## Assumptions:

Data consitency for Country Names that are stored in the database - No duplicate names within the DB as they are used for a Key in the dictionary.
The case is handled gracefully and invalid values are logged

Overseas territories are counted as separate entities. Eg Mayotte's population is not counted towards France mainland population, as national statistics do.

API is stable, and no retry mechanism is needed

## Running the project
1. Build the project 
``dotnet build`` 
(or with VS)

2. Run the code
``dotnet run --project Backend/Backend.csproj``

3. Run the test
``dotnet test Backend.Tests/Backend.Tests.csproj``

(or via Visual studio - build, run, and test explorer)

*``Results output shown in the console``\*

*``Logs are visible under Backend\logs``\*

## Db optimization
Indexes are added to optimize the query execution. On the current dataset they don't make much difference, but could for big data sets
``CREATE INDEX idx_city_stateid ON City(StateId);
CREATE INDEX idx_state_countryid ON State(CountryId);``

##  Potential Improvements / future developments
To make the project more "production ready", a map could be created to store all countries to ISO codes, as this guarantees the unique values accross the datasets. This would then affect the NormalizationService.

Current normalization's 
Configuration could be enriched from a large data set or an external library can be used.

If API is unstable, retries and handling can be implemented through HttpService










