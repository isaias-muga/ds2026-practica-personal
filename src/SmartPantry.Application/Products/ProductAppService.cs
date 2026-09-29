using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace SmartPantry.Products;

public class ProductAppService : SmartPantryAppService, IProductAppService
{
    private readonly IRepository<Product, Guid> _productRepository;

    public ProductAppService(IRepository<Product, Guid> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        // The Id is generated internally using the IGuidGenerator provided by the base class
        var product = new Product(
            GuidGenerator.Create(),
            input.Name
        );

        await _productRepository.InsertAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<ProductDto> GetAsync(Guid id)
    {
        // GetAsync automatically throws 404 if the Id doesn't exist
        var product = await _productRepository.GetAsync(id);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }
}