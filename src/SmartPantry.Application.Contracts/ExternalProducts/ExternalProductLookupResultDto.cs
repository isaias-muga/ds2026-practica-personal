namespace SmartPantry.ExternalProducts;

public class ExternalProductLookupResultDto
{
    public ExternalProductLookupStatus Status { get; set; }

    // Solo vienen cargados cuando Status == Found.
    // Si el proveedor no informó un dato, queda en null (RF-09): nunca se inventa.
    public string? Barcode { get; set; }
    public string? Name { get; set; }
    public string? Brand { get; set; }
    public string? Quantity { get; set; }
    public string? ImageUrl { get; set; }
}