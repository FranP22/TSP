namespace TSP.Common.Dto.App.Cart
{
    public class CartItemValidated : CartItemDefinition
    {
        public string ProductName { get; set; }
        public float ProductPrice { get; set; }
        public bool Valid { get; set; }
    }
}
