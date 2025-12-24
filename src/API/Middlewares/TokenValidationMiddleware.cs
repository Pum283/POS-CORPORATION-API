using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace API;

public class TokenValidationMiddleware : IMiddleware
{
    private readonly IAppConfiguration _configuration;

    public TokenValidationMiddleware(IAppConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        // Sau khi authentication chạy, kiểm tra kết quả
        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized &&
            !context.User.Identity!.IsAuthenticated)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..];

                try
                {
                    var jwtSettings = _configuration.GetJwtSetting();
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                    //var principal = JwtHelper.ValidateJwtToken(token, validationParameters, out var validatedToken);
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                }
                catch (SecurityTokenExpiredException)
                {
                    throw new TokenExpiredException("Token expired");
                }
                catch
                {
                    throw new TokenInvalidException("Token invalid");
                }
            }

            throw new NoAuthenticationHeaderException();
        }
    }
}
