using System.Text.Json.Serialization;

namespace TSP.Common.Dto.Security
{
    public class TokenPair
    {
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
    }
}
