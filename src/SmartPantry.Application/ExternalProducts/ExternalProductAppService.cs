using System.Threading.Tasks;

namespace SmartPantry.ExternalProducts;

public class ExternalProductAppService : SmartPantryAppService, IExternalProductAppService
{
    private readonly IExternalProductCatalogClient _catalogClient;

    public ExternalProductAppService(IExternalProductCatalogClient catalogClient)
    {
        _catalogClient = catalogClient;
    }

    public async Task<ExternalProductLookupResultDto> GetByBarcodeAsync(GetExternalProductInput input)
    {
        try
        {
            var product = await _catalogClient.GetByBarcodeAsync(input.Barcode);

            if (product is null)
            {
                return new ExternalProductLookupResultDto { Status = ExternalProductLookupStatus.NotFound };
            }

            return new ExternalProductLookupResultDto
            {
                Status = ExternalProductLookupStatus.Found,
                Barcode = product.Barcode,
                Name = product.Name,
                Brand = product.Brand,
                Quantity = product.Quantity,
                ImageUrl = product.ImageUrl
            };
        }
        catch (ExternalProductProviderRateLimitedException)
        {
            return new ExternalProductLookupResultDto { Status = ExternalProductLookupStatus.RateLimited };
        }
        catch (ExternalProductProviderUnavailableException)
        {
            return new ExternalProductLookupResultDto { Status = ExternalProductLookupStatus.ProviderUnavailable };
        }
    }
}