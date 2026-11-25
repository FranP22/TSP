namespace TSP.Mediator.Dto.Response.Products
{
    public class ProductResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; } = 0;
        public bool IsActive { get; set; }
        public bool InStock { get; set; }
        public string StockMessage { get; set; } = string.Empty;
    }
}
