using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification
{
    [Fact]
    public void ReturnsAllCatalogItems()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null);

        var result = spec.Evaluate(GetTestCollection());

        Assert.NotNull(result);
        Assert.Equal(4, result.ToList().Count);
    }

    [Fact]
    public void Returns2CatalogItemsWithSameBrandAndTypeId()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, 1, 1);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.ToList().Count);
    }

    [Fact]
    public void ReturnsItemsWithinPriceRange()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null, 1.25m, 2.50m);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // 1.50 and 2.00
    }

    [Fact]
    public void ReturnsItemsWithMinPriceOnly()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null, 2.00m, null);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // 2.00 and 3.00
    }

    [Fact]
    public void ReturnsItemsWithMaxPriceOnly()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null, null, 1.50m);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count); // 1.00 and 1.50
    }

    private List<CatalogItem> GetTestCollection()
    {
        var catalogItemList = new List<CatalogItem>();

        catalogItemList.Add(new CatalogItem(1, 1, "Item 1", "Item 1", 1.00m, "TestUri1"));
        catalogItemList.Add(new CatalogItem(1, 1, "Item 1.5", "Item 1.5", 1.50m, "TestUri1"));
        catalogItemList.Add(new CatalogItem(2, 2, "Item 2", "Item 2", 2.00m, "TestUri2"));
        catalogItemList.Add(new CatalogItem(3, 3, "Item 3", "Item 3", 3.00m, "TestUri3"));

        return catalogItemList;
    }
}
