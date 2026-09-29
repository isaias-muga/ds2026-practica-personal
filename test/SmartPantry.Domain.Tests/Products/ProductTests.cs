using Shouldly;
using System;
using Xunit;

namespace SmartPantry.Products;

public class ProductTests
{
    [Fact]
    public void Should_Create_Valid_Product_And_Normalize_Text()
    {
        var product = new Product(Guid.NewGuid(), "  Test Product  ");
        product.Name.ShouldBe("Test Product");
    }

    [Fact]
    public void Should_Not_Allow_Empty_Or_Whitespace_Name()
    {
        Should.Throw<ArgumentException>(() => new Product(Guid.NewGuid(), "   "));
    }
}