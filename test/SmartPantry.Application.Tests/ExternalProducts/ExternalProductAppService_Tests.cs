using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace SmartPantry.ExternalProducts;

// Plain unit tests: no ABP test base, no database, no Internet.
// The external catalog is replaced by an NSubstitute mock.
public class ExternalProductAppService_Tests
{
    private readonly IExternalProductCatalogClient _catalogClient;
    private readonly ExternalProductAppService _appService;

    public ExternalProductAppService_Tests()
    {
        _catalogClient = Substitute.For<IExternalProductCatalogClient>();
        _appService = new ExternalProductAppService(_catalogClient);
    }

    [Fact]
    public async Task Should_Return_Found_With_Data_When_Product_Exists()
    {
        _catalogClient.GetByBarcodeAsync("3017620422003").Returns(Task.FromResult<ExternalProductDto?>(
            new ExternalProductDto
            {
                Barcode = "3017620422003",
                Name = "Nutella",
                Brand = "Nutella, Ferrero",
                Quantity = "400 g",
                ImageUrl = "https://images.openfoodfacts.org/front.jpg"
            }));

        var result = await _appService.GetByBarcodeAsync(new GetExternalProductInput { Barcode = "3017620422003" });

        result.Status.ShouldBe(ExternalProductLookupStatus.Found);
        result.Barcode.ShouldBe("3017620422003");
        result.Name.ShouldBe("Nutella");
        result.Brand.ShouldBe("Nutella, Ferrero");
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Product_Does_Not_Exist()
    {
        _catalogClient.GetByBarcodeAsync("0000000000000").Returns(Task.FromResult<ExternalProductDto?>(null));

        var result = await _appService.GetByBarcodeAsync(new GetExternalProductInput { Barcode = "0000000000000" });

        result.Status.ShouldBe(ExternalProductLookupStatus.NotFound);
        result.Name.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Keep_Missing_Fields_As_Null_When_Provider_Data_Is_Incomplete()
    {
        _catalogClient.GetByBarcodeAsync("7790000000001").Returns(Task.FromResult<ExternalProductDto?>(
            new ExternalProductDto { Barcode = "7790000000001", Name = "Yerba" })); // no brand, quantity or image

        var result = await _appService.GetByBarcodeAsync(new GetExternalProductInput { Barcode = "7790000000001" });

        result.Status.ShouldBe(ExternalProductLookupStatus.Found);
        result.Name.ShouldBe("Yerba");
        result.Brand.ShouldBeNull();      // RF-09: absent, not invented
        result.Quantity.ShouldBeNull();
        result.ImageUrl.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Return_RateLimited_When_Provider_Throttles()
    {
        _catalogClient.GetByBarcodeAsync(Arg.Any<string>())
            .ThrowsAsync(new ExternalProductProviderRateLimitedException());

        var result = await _appService.GetByBarcodeAsync(new GetExternalProductInput { Barcode = "3017620422003" });

        result.Status.ShouldBe(ExternalProductLookupStatus.RateLimited);
    }

    [Fact]
    public async Task Should_Return_ProviderUnavailable_When_Provider_Fails()
    {
        _catalogClient.GetByBarcodeAsync(Arg.Any<string>())
            .ThrowsAsync(new ExternalProductProviderUnavailableException("Open Food Facts did not respond in time."));

        var result = await _appService.GetByBarcodeAsync(new GetExternalProductInput { Barcode = "3017620422003" });

        result.Status.ShouldBe(ExternalProductLookupStatus.ProviderUnavailable);
    }
}