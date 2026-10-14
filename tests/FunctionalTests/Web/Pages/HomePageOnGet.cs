using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests.Web.Pages;

[Collection("Sequential")]
public class HomePageOnGet(TestApplication factory) : IClassFixture<TestApplication>
{
    public HttpClient Client { get; } = factory.CreateClient();

    [Fact]
    public async Task ReturnsHomePageWithProductListing()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(".NET Bot Black Sweatshirt", stringResponse);
    }

    [Fact]
    public async Task FiltersOutItemsAbovePriceMax()
    {
        // The seeded ".NET Bot Black Sweatshirt" costs 19.50, so a max of 15 excludes it.
        var response = await Client.GetAsync("/?PriceMaxApplied=15", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.DoesNotContain(".NET Bot Black Sweatshirt", stringResponse);
    }

    [Fact]
    public async Task IncludesItemsWithinPriceRange()
    {
        // 19.50 falls inside [15, 25].
        var response = await Client.GetAsync("/?PriceMinApplied=15&PriceMaxApplied=25", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains(".NET Bot Black Sweatshirt", stringResponse);
    }
}
