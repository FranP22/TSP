namespace TSP.Common.Dto.Db
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoleId { get; set; }
        public string Email { get; set; } = String.Empty;
        public string EmailNormalized { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public bool IsActive { get; set; } = true;
        public int AuthVersion { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppRole Role { get; set; } = null!;
        public ICollection<AppUserSession> Sessions { get; set; } = new List<AppUserSession>();
        public ICollection<AppOrder> Orders { get; set; } = new List<AppOrder>();
    }
}
