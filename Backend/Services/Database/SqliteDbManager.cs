using System;
using System.Data.Common;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Backend.Services.Database;

public class SqliteDbManager : IDbManager
{
	public DbConnection GetConnection()
	{
		try
		{
			var dbPath = GetDatabasePath();
			var connection = new SqliteConnection($"Data Source={dbPath};Mode=ReadWrite");
			connection.Open();
			return connection;
		}
		catch (SqliteException ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	// adding this so the project runs from the root directory with dotnet run --project Backend/Backend.csproj
	private static string GetDatabasePath()
	{
		var dbFileName = "citystatecountry.db";

		// Try current directory first
		if (File.Exists(dbFileName))
			return dbFileName;

		// Try Backend subdirectory (for running from parent directory)
		var backendPath = Path.Combine("Backend", dbFileName);
		if (File.Exists(backendPath))
			return backendPath;

		// Default
		return dbFileName;
	}
}
