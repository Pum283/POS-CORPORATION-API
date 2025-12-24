namespace API.Controllers;

[Route("auth")]
public class AuthsController(
    IUsersService _usersService,
    IAuthService _authService,
    IAppConfiguration _configuration,
    IMapper _mapper) : BaseApiController
{
    [HttpPost("login")]
    public async Task<TokenResponse> Login([FromBody] UserLoginModel model)
    {
        var jwtSettings = _configuration.GetJwtSetting();
        var user = await _usersService.LoginAsync(model);
        var accessToken = await _authService.GenerateAccessToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        await _usersService.UpdateRefreshTokenAsync(user.Id, refreshToken);

        return new TokenResponse
        {
            AccessToken = accessToken,
            ExpiresIn = jwtSettings.AccessTokenExpiration * 60
        };
    }
    [HttpPost("v2/refresh-token")]
    public async Task<TokenResponse> RefreshToken(RefreshTokenModel model)
    {
        var jwtSettings = _configuration.GetJwtSetting();
        var principal = _authService.GetPrincipalFromExpiredToken(model.Token);
        var userId = principal.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;

        var userWithRefreshToken = await _usersService.GetByIdForRefreshTokenAsync(Guid.Parse(userId ?? ""));

        var user = _mapper.Map<User>(userWithRefreshToken);
        var newAccessToken = await _authService.GenerateAccessToken(user);
        var newRefreshToken = _authService.GenerateRefreshToken();

        await _usersService.UpdateRefreshTokenAsync(user.Id, newRefreshToken);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            ExpiresIn = jwtSettings.AccessTokenExpiration * 60
        };
    }
    [HttpPost("logout")]
    public async Task<OperationResult> Logout(RefreshTokenModel model)
    {
        var principal = _authService.GetPrincipalFromExpiredToken(model.Token);
        var userId = principal.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var revomeRefreshToken = "";

        await _usersService.UpdateRefreshTokenAsync(Guid.Parse(userId!), revomeRefreshToken);

        return new OperationResult
        {
            Success = true,
            Message = "Logout successful"
        };
    }
}
