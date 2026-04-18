using Backend.Services.CountryNormalization;
using System.Text.Json;

namespace Backend.Tests;


[Collection("CountryNormalization")]
public class CountryNormalizationInvalidConfigTests
{
    private static string RuntimeConfigPath => Path.Combine(AppContext.BaseDirectory, "Config", "CountryMappingConfig.json");

    [Fact]
    public void Ctor_ThrowsJsonException_WhenConfigIsMalformed()
    {
        var configPath = RuntimeConfigPath;
        var directory = Path.GetDirectoryName(configPath)!;
        Directory.CreateDirectory(directory);
        var hadOriginal = File.Exists(configPath);
        var originalContent = hadOriginal ? File.ReadAllText(configPath) : null;

        File.WriteAllText(configPath, "{ invalid-json }");

        try
        {
            Assert.Throws<JsonException>(() => new CountryNormalizationService());
        }
        finally
        {
            if (hadOriginal)
            {
                File.WriteAllText(configPath, originalContent!);
            }
            else if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }

    [Fact]
    public void Ctor_ThrowsFileNotFoundException_WhenConfigIsMissing()
    {
        var configPath = RuntimeConfigPath;
        var hadOriginal = File.Exists(configPath);
        var originalContent = hadOriginal ? File.ReadAllText(configPath) : null;

        try
        {
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }

            Assert.Throws<FileNotFoundException>(() => new CountryNormalizationService());
        }
        finally
        {
            if (hadOriginal)
            {
                var directory = Path.GetDirectoryName(configPath)!;
                Directory.CreateDirectory(directory);
                File.WriteAllText(configPath, originalContent!);
            }
        }
    }
}