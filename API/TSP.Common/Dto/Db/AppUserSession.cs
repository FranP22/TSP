namespace TSP.Common.Dto.Db
{
    public class AppUserSession
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string RefreshTokenHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastRotatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokeReason { get; set; } = string.Empty;

        public AppUser User { get; set; } = null!;
        public ICollection<AppUsedRefreshTokens> UsedRefreshTokens { get; set; } = new List<AppUsedRefreshTokens>();
    }
}
