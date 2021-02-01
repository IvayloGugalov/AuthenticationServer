using System.ComponentModel.DataAnnotations;

namespace AuthenticationServer.API.Models.Request
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshTokenValue { get; set; }
    }
}
