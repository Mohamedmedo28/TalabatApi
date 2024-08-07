using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductSpecifications:BaseSpecifications<Product>
    {
     

       // public ProductSpecifications(string? sort, int? BrandId, int? TypeId)
        public ProductSpecifications(ProductSpecParams productSpec)
        : base(p =>
             (string.IsNullOrEmpty(productSpec.Search) || p.Name.ToLower().Contains(productSpec.Search)) &&
            (!productSpec.BrandId.HasValue || p.ProductBrandId == productSpec.BrandId.Value) &&
            (!productSpec.TypeId.HasValue || p.ProductTypeId == productSpec.TypeId.Value)
        )
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);

            if (!string.IsNullOrEmpty(productSpec.sort))
            {
                switch (productSpec.sort)
                {
                    case "priceAsc":
                        //OrderBy = p => p.Price;
                        addOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                       // OrderByDescending = p => p.Price;
                        addOrderByDescending(p => p.Price);
                        break;
                    default:
                        addOrderBy(p=>p.Name);
                        break;
                }
            }

            //totalProducts = 100
            //PageSize = 10
            //PageIndex = 3
            // ( 10*(3-1),10) => 2 Skip 
            ApplyPagination(productSpec.PageSize * (productSpec.PageIndex - 1), productSpec.PageSize);
        }
        public ProductSpecifications(int id):base(p=>p.Id == id)
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);
        }
    }
}
