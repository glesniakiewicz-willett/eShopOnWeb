using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CatalogFilterSpecification
{
    [Theory]
    [InlineData(null, null, 5)]
    [InlineData(1, null, 3)]
    [InlineData(2, null, 2)]
    [InlineData(null, 1, 2)]
    [InlineData(null, 3, 1)]
    [InlineData(1, 3, 1)]
    [InlineData(2, 3, 0)]
    public void MatchesExpectedNumberOfItems(int? brandId, int? typeId, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId);

        var result = spec.Evaluate(GetTestItemCollection()).ToList();

        Assert.Equal(expectedCount, result.Count());
    }

    [Theory]
    [InlineData(null, null, 5)] // no price filter
    [InlineData(25.0, null, 3)] // min only
    [InlineData(null, 25.0, 2)] // max only
    [InlineData(20.0, 40.0, 3)] // both sides
    [InlineData(50.0, null, 1)] // min at the top boundary
    [InlineData(35.0, 15.0, 0)] // min greater than max
    public void MatchesExpectedNumberOfItemsByPrice(double? priceMin, double? priceMax, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(null, null, (decimal?)priceMin, (decimal?)priceMax);

        var result = spec.Evaluate(GetTestItemCollectionWithPrices()).ToList();

        Assert.Equal(expectedCount, result.Count);
    }

    public List<CatalogItem> GetTestItemCollection()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(2, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(3, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(1, 2, "Description", "Name", 0, "FakePath"),
                new CatalogItem(2, 2, "Description", "Name", 0, "FakePath"),
            };
    }

    public List<CatalogItem> GetTestItemCollectionWithPrices()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Name", 10m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Name", 20m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Name", 30m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Name", 40m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Name", 50m, "FakePath"),
            };
    }
}
