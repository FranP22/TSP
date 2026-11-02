namespace TSP.Common.Dto.Db
{
    public class AppUsedRefreshTokens
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SessionId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        public AppUserSession UserSession { get; set; } = null!;
    }
}
