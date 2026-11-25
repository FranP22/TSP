using System.ComponentModel.DataAnnotations;

namespace TSP.Common.Dto.App.Cart
{
    public class CartItemDefinition
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        public int ProductCount { get; set; }
    }
}
