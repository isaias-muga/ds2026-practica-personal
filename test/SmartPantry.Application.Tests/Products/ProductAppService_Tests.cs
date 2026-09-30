using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
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

    [Fact]
    public async Task Should_Create_List_Update_Get_And_Delete_Product()
    {
        var created = await _productAppService.CreateAsync(new CreateProductDto { Name = "Whole milk 1L" });

        var list = await _productAppService.GetListAsync(new PagedAndSortedResultRequestDto());
        list.Items.ShouldContain(p => p.Id == created.Id);

        var updated = await _productAppService.UpdateAsync(created.Id, new UpdateProductDto { Name = "Skim milk 1L" });
        updated.Name.ShouldBe("Skim milk 1L");

        var fetched = await _productAppService.GetAsync(created.Id);
        fetched.Name.ShouldBe("Skim milk 1L");

        await _productAppService.DeleteAsync(created.Id);

        await Should.ThrowAsync<EntityNotFoundException>(async () =>
            await _productAppService.GetAsync(created.Id));
    }
}