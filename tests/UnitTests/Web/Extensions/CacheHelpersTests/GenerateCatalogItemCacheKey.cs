using Microsoft.eShopWeb.Web;
using Microsoft.eShopWeb.Web.Extensions;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.Web.Extensions.CacheHelpersTests;

public class GenerateCatalogItemCacheKey
{
    [Fact]
    public void ReturnsCatalogItemCacheKey()
    {
        var pageIndex = 0;
        int? brandId = null;
        int? typeId = null;

        var result = CacheHelpers.GenerateCatalogItemCacheKey(pageIndex, Constants.ITEMS_PER_PAGE, brandId, typeId);

        Assert.Equal("items-0-10----", result);
    }

    [Fact]
    public void IncludesPriceRangeInCacheKey()
    {
        var result = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, 10m, 50m);

        Assert.Equal("items-0-10---10-50", result);
    }

    [Fact]
    public void DifferentPriceRangesProduceDifferentKeys()
    {
        var keyA = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, 10m, 50m);
        var keyB = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, 20m, 60m);

        Assert.NotEqual(keyA, keyB);
    }
}
