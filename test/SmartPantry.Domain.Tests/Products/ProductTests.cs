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

    [Fact]
    public void Should_Update_Name_And_Normalize_Text()
    {
        var product = new Product(Guid.NewGuid(), "Whole milk 1L");

        product.SetName("  Skim milk 1L  ");

        product.Name.ShouldBe("Skim milk 1L");
    }

    [Fact]
    public void Should_Reject_Invalid_Update_And_Leave_Entity_Unchanged()
    {
        var product = new Product(Guid.NewGuid(), "Whole milk 1L");

        Should.Throw<ArgumentException>(() => product.SetName("   "));

        product.Name.ShouldBe("Whole milk 1L"); // no cambió
    }
}