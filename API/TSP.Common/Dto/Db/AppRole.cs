namespace TSP.Common.Dto.Db
{
    public class AppRole
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = String.Empty;
        public string NameNormalized { get; set; } = String.Empty;
        public bool IsDefault { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<AppRoleClaim> Claims { get; set; } = new List<AppRoleClaim>();
    }
}
