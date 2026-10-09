using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartPantry.ExternalProducts;

public class OpenFoodFactsProductCatalogClient : IExternalProductCatalogClient
{
    // Ask only for what the system uses — never the whole product document.
    private const string Fields = "code,product_name,product_name_es,brands,quantity,image_front_url";

    private readonly HttpClient _httpClient;

    // HttpClient arrives already configured (base URL, timeout, User-Agent)
    // by IHttpClientFactory — see step 5. Never `new HttpClient()` here.
    public OpenFoodFactsProductCatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ExternalProductDto?> GetByBarcodeAsync(string barcode)
    {
        // Relative path, no leading "/": keeps the "api/v3/" from the BaseAddress.
        var path = $"product/{Uri.EscapeDataString(barcode)}?fields={Uri.EscapeDataString(Fields)}";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(path);
        }
        catch (TaskCanceledException ex)
        {
            throw new ExternalProductProviderUnavailableException("Open Food Facts did not respond in time.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalProductProviderUnavailableException("Could not connect to Open Food Facts.", ex);
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new ExternalProductProviderRateLimitedException();
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalProductProviderUnavailableException(
                    $"Open Food Facts returned HTTP {(int)response.StatusCode}.");
            }

            OpenFoodFactsProductResponse? payload;
            try
            {
                payload = await response.Content.ReadFromJsonAsync<OpenFoodFactsProductResponse>();
            }
            catch (JsonException ex)
            {
                throw new ExternalProductProviderUnavailableException(
                    "Open Food Facts returned a response that could not be read.", ex);
            }

            var product = payload?.Product;
            if (product is null)
            {
                return null;
            }

            // Translation into OUR model. Missing values stay null — never invented (RF-09).
            return new ExternalProductDto
            {
                Barcode = NullIfBlank(product.Code) ?? barcode,
                Name = NullIfBlank(product.ProductNameEs) ?? NullIfBlank(product.ProductName),
                Brand = NullIfBlank(product.Brands),
                Quantity = NullIfBlank(product.Quantity),
                ImageUrl = NullIfBlank(product.ImageFrontUrl)
            };
        }
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

// Internal deserialization models: only the requested fields, mapped from the provider's
// snake_case names. `internal` = invisible outside HttpApi.Host, so they can never become
// part of SmartPantry's public contract.
internal sealed class OpenFoodFactsProductResponse
{
    [JsonPropertyName("product")]
    public OpenFoodFactsProduct? Product { get; set; }
}

internal sealed class OpenFoodFactsProduct
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("product_name_es")]
    public string? ProductNameEs { get; set; }

    [JsonPropertyName("brands")]
    public string? Brands { get; set; }

    [JsonPropertyName("quantity")]
    public string? Quantity { get; set; }

    [JsonPropertyName("image_front_url")]
    public string? ImageFrontUrl { get; set; }
}