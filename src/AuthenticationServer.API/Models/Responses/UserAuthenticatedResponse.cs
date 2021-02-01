
namespace AuthenticationServer.API.Models.Responses
{
    public class UserAuthenticatedResponse
    {
        public string AccessTokenValue { get; set; }
        public string RefreshTokenValue { get; set; }
    }
}
