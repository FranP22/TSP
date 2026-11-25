using System.ComponentModel.DataAnnotations;
using TSP.Common.Dto.App.Cart;

namespace TSP.Mediator.Dto.Request.Orders
{
    public class PlaceOrderRequestDto
    {
        [Required]
        public List<CartItemDefinition> Items { get; set; }
    }
}
