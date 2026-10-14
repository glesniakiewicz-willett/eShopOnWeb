using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class CatalogFilterSpecification : Specification<CatalogItem>
{
    public CatalogFilterSpecification(int? brandId, int? typeId, decimal? priceMin = null, decimal? priceMax = null)
    {
        Query.Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId) &&
            (!priceMin.HasValue || i.Price >= priceMin.Value) &&
            (!priceMax.HasValue || i.Price <= priceMax.Value));
    }
}
