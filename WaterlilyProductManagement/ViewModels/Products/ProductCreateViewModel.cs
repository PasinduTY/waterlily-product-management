using System.ComponentModel.DataAnnotations;

namespace WaterlilyProductManagement.ViewModels.Products;

public class ProductCreateViewModel
{
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required.")]
    [Range(typeof(decimal), "0.01", "9999999999999999.99",
        ErrorMessage = "Price must be greater than zero.")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Stock is required.")]
    [Range(0, int.MaxValue,
        ErrorMessage = "Stock cannot be negative.")]
    public int? Stock { get; set; }
}
