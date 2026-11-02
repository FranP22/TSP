namespace TSP.Common.Dto.Db
{
    public class AppOrder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public float Total { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; } = null!;
    }
}
