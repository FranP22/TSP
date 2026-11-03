using System.ComponentModel.DataAnnotations;

namespace TSP.Mediator.Dto.Request.Auth
{
    public class LoginRequestDto
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
