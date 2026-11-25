using System.ComponentModel.DataAnnotations;

namespace TSP.Mediator.Dto.Request.Products
{
    public class CreateProductRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; } = 0;
        [Required]
        [Range(0, float.MaxValue)]
        public int StockQuantity { get; set; } = 0;
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
