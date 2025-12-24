namespace Application;

public interface IAuthService
{
    Task<string> GenerateAccessToken(User user);
    string GenerateRefreshToken();
    //Task<User> GetUserByRefreshTokenAsync(Guid id);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
