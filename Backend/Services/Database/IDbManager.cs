using System.Data.Common;

namespace Backend.Services.Database;

public interface IDbManager
{
    DbConnection GetConnection();
}
