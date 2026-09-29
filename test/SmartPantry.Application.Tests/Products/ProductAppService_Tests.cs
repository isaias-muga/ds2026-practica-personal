using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace SmartPantry.Products;

public abstract class ProductAppService_Tests<TStartupModule> : SmartPantryApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IProductAppService _productAppService;

    protected ProductAppService_Tests()
    {
        _productAppService = GetRequiredService<IProductAppService>();
    }

    [Fact]
    public async Task Should_Create_And_Then_Get_Product_By_Id()
    {
        var created = await _productAppService.CreateAsync(new CreateProductDto
        {
            Name = "Test Product"
        });

        created.Id.ShouldNotBe(Guid.Empty);

        var fetched = await _productAppService.GetAsync(created.Id);
        fetched.Name.ShouldBe("Test Product");
    }

    [Fact]
    public async Task Should_Not_Create_Product_Without_Name()
    {
        await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _productAppService.CreateAsync(new CreateProductDto { Name = "" });
        });
    }
}