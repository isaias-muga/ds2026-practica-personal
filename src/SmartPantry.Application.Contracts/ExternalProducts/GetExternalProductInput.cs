using System.ComponentModel.DataAnnotations;

namespace SmartPantry.ExternalProducts;

public class GetExternalProductInput
{
    [Required]
    [RegularExpression(@"^\d{8,14}$", ErrorMessage = "The barcode must contain between 8 and 14 digits.")]
    public string Barcode { get; set; } = null!;
}