namespace TSP.Common.Dto.Db
{
    public class AppRoleClaim
    {
        public string RoleId { get; set; }
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;

        public AppRole Role { get; set; } = null!;
    }
}
