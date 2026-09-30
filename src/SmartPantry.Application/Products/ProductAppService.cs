using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace SmartPantry.Products;

public class ProductAppService :
    CrudAppService<Product, ProductDto, Guid, PagedAndSortedResultRequestDto, CreateProductDto, UpdateProductDto>,
    IProductAppService
{
    public ProductAppService(IRepository<Product, Guid> repository) : base(repository)
    {
    }

    protected override Task<Product> MapToEntityAsync(CreateProductDto createInput)
    {
        return Task.FromResult(new Product(GuidGenerator.Create(), createInput.Name));
    }

    protected override Task MapToEntityAsync(UpdateProductDto updateInput, Product entity)
    {
        entity.SetName(updateInput.Name);
        return Task.CompletedTask;
    }
}