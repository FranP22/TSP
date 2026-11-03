using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Security;
using TSP.Common.Helper;
using TSP.Data;
using TSP.Services.Service.Interface;

namespace TSP.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly ITokenService _token;

        public AuthService(AppDbContext db, ITokenService token)
        {
            _db = db;
            _token = token;
        }

        public async Task<TokenPair?> LoginAsync(string email, string password)
        {
            var user = await _db.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            var hashedPassword = SecurityHelper.CreatePasswordHash(user, password);

            if (user.PasswordHash != hashedPassword) return null;

            return await _token.GenerateTokensAsync(user);
        }
    }
}
