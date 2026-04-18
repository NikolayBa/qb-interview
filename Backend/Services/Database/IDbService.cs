using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services.Database;

public interface IDbService
{
    Task<Dictionary<string, long>> GetCountryPopulationFromDbAsync();
}
