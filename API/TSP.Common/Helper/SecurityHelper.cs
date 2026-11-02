using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using TSP.Common.Dto.Db;

namespace TSP.Common.Helper
{
    public class SecurityHelper
    {
        public static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes);
        }

        public static string Hash(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            var hash = SHA256.HashData(bytes);

            return Convert.ToBase64String(hash);
        }

        public static string CreatePasswordHash(AppUser user, string password)
        {
            var hasher = new PasswordHasher<AppUser>();

            var hashedPassword = hasher.HashPassword(user, password);

            return hashedPassword;
        }
    }
}
