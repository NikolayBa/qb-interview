using Backend.Services.CountryNormalization;

namespace Backend.Tests;

public class CountryNormalizationServiceTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeCountryName_ReturnsEmpty_ForNullOrWhitespace(string? input)
    {
        var sut = new CountryNormalizationService();

        var result = sut.NormalizeCountryName(input!);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void NormalizeCountryName_ReturnsMappedValue_WhenMappingExists()
    {
        var sut = new CountryNormalizationService();

        var result = sut.NormalizeCountryName("United States of America");

        Assert.Equal("U.S.A.", result);
    }

    [Fact]
    public void NormalizeCountryName_ReturnsOriginal_WhenMappingDoesNotExist()
    {
        var sut = new CountryNormalizationService();

        var result = sut.NormalizeCountryName("India");

        Assert.Equal("India", result);
    }
}
