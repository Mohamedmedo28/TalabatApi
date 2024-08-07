using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Error;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : ApiBaseController
    {
        private readonly IUnitOfWork unitOfWork;

        //private readonly IGenericRepository<Product> productRepo;
        //private readonly IGenericRepository<ProductBrand> brandRepo;
        //private readonly IGenericRepository<ProductType> typeRepo;
        private readonly IMapper mapper;

        public ProductsController(
            //IGenericRepository<Product> ProductRepo ,
            //IGenericRepository<ProductBrand> brandRepo ,
            //IGenericRepository<ProductType> typeRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            //productRepo = ProductRepo;
            //this.brandRepo = brandRepo;
            //this.typeRepo = typeRepo;
            this.mapper = mapper;
        }
        [Authorize]
        [HttpGet]
       // public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(string? sort,int? BrandId ,int? TypeId)
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams productSpec)
        {
            var spec = new ProductSpecifications(productSpec);
            var Products = await unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec );

            //var mappedProducts = mapper.Map<IReadOnlyList<Product> , IReadOnlyList<ProductToReturnDto>>(Products);
            var Data = mapper.Map<IReadOnlyList<Product> , IReadOnlyList<ProductToReturnDto>>(Products);

            var CounTSpec = new ProductWithFilterationForCountSpecification(productSpec);
            var Count = await unitOfWork.Repository<Product>().GetCountWithSpecAsync(CounTSpec);
            return Ok(new Pagination<ProductToReturnDto>(productSpec.PageSize,productSpec.PageIndex,Data,Count)); 
        }
        [ProducesResponseType(typeof(ProductToReturnDto) ,StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse) ,StatusCodes.Status404NotFound)]

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec= new ProductSpecifications(id);
            var product = await unitOfWork.Repository<Product>().GetWithSpecAsync(spec);

            var mappedProducts = mapper.Map<Product, ProductToReturnDto>(product);
            return Ok(mappedProducts);  
        }

        [HttpGet("brand")] //api/products/brand
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrand()
        {
            var brand = await unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brand);

        }

        [HttpGet("type")] //api/products/type
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetType()
        {
            var type = await unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(type);

        }
    }
}
