namespace AuthenticationServer.API.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(this string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(this string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
