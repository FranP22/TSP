namespace TSP.Common.Dto.Db
{
    public class AppProduct
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
