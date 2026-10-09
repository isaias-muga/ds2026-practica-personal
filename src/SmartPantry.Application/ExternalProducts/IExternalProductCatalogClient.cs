using System.Threading.Tasks;

namespace SmartPantry.ExternalProducts;

// "I need to look something up in an external catalog."
// Knows nothing about HttpClient or Open Food Facts.
public interface IExternalProductCatalogClient
{
    // null = the product doesn't exist in the external catalog.
    Task<ExternalProductDto?> GetByBarcodeAsync(string barcode);
}